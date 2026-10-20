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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpPost("events/{eventId}/results")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> CaptureResult(int eventId, CreateResultDto dto)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null) return NotFound("Event not found.");
            if (eventItem.OrganiserId != organiserId) return Forbid("You do not own this event.");

            // Verify the enrollment exists and belongs to a category in this event
            var enrollment = await _context.Enrollments
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EnrollmentId == dto.EnrollmentId && e.Category.EventId == eventId);

            if (enrollment == null) return BadRequest("Invalid enrollment for this event.");

            // Check if result already exists
            if (await _context.Results.AnyAsync(r => r.EnrollmentId == dto.EnrollmentId))
                return BadRequest("Result already captured for this enrollment.");

            var result = new Result
            {
                EnrollmentId = dto.EnrollmentId,
                FinishTime = dto.FinishTime,
                Position = dto.Position
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Result captured successfully" });
        }

        [HttpGet("results/me")]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var results = await _context.Results
                .Include(r => r.Enrollment)
                .ThenInclude(e => e.Category)
                .ThenInclude(c => c.Event)
                .Where(r => r.Enrollment.UserId == userId)
                .Select(r => new
                {
                    r.ResultId,
                    EventName = r.Enrollment.Category.Event.Name,
                    CategoryName = r.Enrollment.Category.Name,
                    r.FinishTime,
                    r.Position
                }).ToListAsync();

            return Ok(results);
        }

        [HttpGet("events/{eventId}/results")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            var results = await _context.Results
                .Include(r => r.Enrollment)
                .ThenInclude(e => e.User)
                .Include(r => r.Enrollment.Category)
                .Where(r => r.Enrollment.Category.EventId == eventId)
                .Select(r => new
                {
                    r.ResultId,
                    ParticipantName = r.Enrollment.User.FullName,
                    CategoryName = r.Enrollment.Category.Name,
                    r.FinishTime,
                    r.Position
                }).ToListAsync();

            return Ok(results);
        }
    }
}