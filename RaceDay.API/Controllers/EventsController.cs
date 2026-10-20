using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Attributes;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all upcoming events.
        /// </summary>
        /// <returns>A list of all events.</returns>
        /// <response code="200">Returns the list of events.</response>
        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _context.Events
                .Select(e => new EventDto
                {
                    EventId = e.EventId,
                    Name = e.Name,
                    Description = e.Description,
                    EventDate = e.EventDate,
                    Location = e.Location,
                    Distance = e.Distance,
                    EventType = e.EventType,
                    OrganiserId = e.OrganiserId
                }).ToListAsync();

            return Ok(events);
        }

        /// <summary>
        /// Retrieves a specific event by its ID.
        /// </summary>
        /// <param name="id">The ID of the event.</param>
        /// <returns>The event details.</returns>
        /// <response code="200">Returns the event.</response>
        /// <response code="404">Event not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(int id)
        {
            var e = await _context.Events.FindAsync(id);
            if (e == null) return NotFound();

            return Ok(new EventDto
            {
                EventId = e.EventId,
                Name = e.Name,
                Description = e.Description,
                EventDate = e.EventDate,
                Location = e.Location,
                Distance = e.Distance,
                EventType = e.EventType,
                OrganiserId = e.OrganiserId
            });
        }

        /// <summary>
        /// Creates a new event. Only accessible to Organisers.
        /// </summary>
        /// <param name="dto">The event details.</param>
        /// <returns>The newly created event.</returns>
        /// <response code="201">Event created successfully.</response>
        /// <response code="401">User is not logged in.</response>
        /// <response code="403">User is not an Organiser.</response>
        [HttpPost]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> CreateEvent(CreateUpdateEventDto dto)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            if (organiserId == null) return Unauthorized();

            var newEvent = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                Location = dto.Location,
                Distance = dto.Distance,
                EventType = dto.EventType,
                OrganiserId = organiserId.Value
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = newEvent.EventId }, newEvent);
        }

        /// <summary>
        /// Updates an existing event. Only accessible to the Organiser who created it.
        /// </summary>
        /// <param name="id">The ID of the event to update.</param>
        /// <param name="dto">The updated event details.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">Event updated successfully.</response>
        /// <response code="403">User does not own this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpPut("{id}")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> UpdateEvent(int id, CreateUpdateEventDto dto)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null) return NotFound();
            if (existingEvent.OrganiserId != organiserId) return Forbid("You do not own this event.");

            existingEvent.Name = dto.Name;
            existingEvent.Description = dto.Description;
            existingEvent.EventDate = dto.EventDate;
            existingEvent.Location = dto.Location;
            existingEvent.Distance = dto.Distance;
            existingEvent.EventType = dto.EventType;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Event updated successfully" });
        }

        /// <summary>
        /// Deletes an event. Only accessible to the Organiser who created it.
        /// </summary>
        /// <param name="id">The ID of the event to delete.</param>
        /// <returns>No content.</returns>
        /// <response code="204">Event deleted successfully.</response>
        /// <response code="403">User does not own this event.</response>
        /// <response code="404">Event not found.</response>
        [HttpDelete("{id}")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null) return NotFound();
            if (existingEvent.OrganiserId != organiserId) return Forbid("You do not own this event.");

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}