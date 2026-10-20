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

        /// <summary>
        /// Captures a participant's result for a specific event. Only accessible to the Organiser who created the event.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <param name="dto">The result details (EnrollmentId, FinishTime, Position).</param>
        /// <returns>A success message.</returns>
        /// <response code="200">Result captured successfully.</response>
        /// <response code="400">Invalid enrollment or result already exists.</response>
        /// <response code="403">User does not own this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpPost("events/{eventId}/results")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> CaptureResult(int eventId, CreateResultDto dto)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null) return NotFound("Event not found.");
            if (eventItem.OrganiserId != organiserId) return Forbid("You do not own this event.");

            var enrollment = await _context.Enrollments
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EnrollmentId == dto.EnrollmentId && e.Category!.EventId == eventId);

            if (enrollment == null) return BadRequest("Invalid enrollment for this event.");

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

        /// <summary>
        /// Retrieves all personal results for the currently logged-in participant.
        /// </summary>
        /// <returns>A list of the participant's results.</returns>
        /// <response code="200">Returns the list of results.</response>
        [HttpGet("results/me")]
        [AuthorizeRole("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            var results = await _context.Results
                .Include(r => r.Enrollment)
                .ThenInclude(e => e.Category)
                .ThenInclude(c => c.Event)
                .Where(r => r.Enrollment!.UserId == userId)
                .Select(r => new
                {
                    r.ResultId,
                    EventName = r.Enrollment!.Category!.Event!.Name,
                    // FIXED: Added '!' to Category
                    CategoryName = r.Enrollment.Category!.Name,
                    r.FinishTime,
                    r.Position
                }).ToListAsync();

            return Ok(results);
        }

        /// <summary>
        /// Retrieves all results for a specific event.
        /// </summary>
        /// <param name="eventId">The ID of the event.</param>
        /// <returns>A list of results for the event.</returns>
        /// <response code="200">Returns the list of results.</response>
        [HttpGet("events/{eventId}/results")]
        public async Task<IActionResult> GetEventResults(int eventId)
        {
            var results = await _context.Results
                .Include(r => r.Enrollment)
                .ThenInclude(e => e.User)
                .Include(r => r.Enrollment!.Category)
                .Where(r => r.Enrollment!.Category!.EventId == eventId)
                .Select(r => new
                {
                    r.ResultId,
                    ParticipantName = r.Enrollment!.User!.FullName,
                    CategoryName = r.Enrollment.Category!.Name,
                    r.FinishTime,
                    r.Position
                }).ToListAsync();

            return Ok(results);
        }
    }
}