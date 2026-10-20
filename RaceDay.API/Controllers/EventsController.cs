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