using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Attributes;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrollmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpPost("events/{eventId}/enroll")]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> EnrollInEvent(int eventId, CreateEnrollmentDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            // Verify the category exists and belongs to the event
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == dto.CategoryId && c.EventId == eventId);
            if (category == null) return BadRequest("Invalid category for this event.");

            // Check if user is already enrolled in this category
            var existingEnrollment = await _context.Enrollments
                .AnyAsync(e => e.UserId == userId && e.CategoryId == dto.CategoryId);

            if (existingEnrollment) return BadRequest("You are already enrolled in this category.");

            var enrollment = new Enrollment
            {
                UserId = userId.Value,
                CategoryId = dto.CategoryId,
                Status = "Confirmed"
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Successfully enrolled" });
        }

        [HttpGet("enrollments/me")]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var enrollments = await _context.Enrollments
                .Include(e => e.Category)
                .ThenInclude(c => c.Event)
                .Where(e => e.UserId == userId)
                .Select(e => new EnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    EnrollmentDate = e.EnrollmentDate,
                    Status = e.Status,
                    EventName = e.Category.Event.Name,
                    CategoryName = e.Category.Name
                }).ToListAsync();

            return Ok(enrollments);
        }

        [HttpDelete("enrollments/{id}")]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> CancelEnrollment(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null) return NotFound();
            if (enrollment.UserId != userId) return Forbid("You can only cancel your own enrollment.");

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}