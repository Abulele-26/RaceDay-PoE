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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("events/{eventId}/categories")]
        public async Task<IActionResult> GetCategoriesForEvent(int eventId)
        {
            var categories = await _context.Categories
                .Where(c => c.EventId == eventId)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Distance = c.Distance,
                    EntryFee = c.EntryFee,
                    EventId = c.EventId
                }).ToListAsync();

            return Ok(categories);
        }

        [HttpPost("events/{eventId}/categories")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> CreateCategory(int eventId, CreateUpdateCategoryDto dto)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            var eventItem = await _context.Events.FindAsync(eventId);

            if (eventItem == null) return NotFound("Event not found.");
            if (eventItem.OrganiserId != organiserId) return Forbid("You do not own this event.");

            var category = new Category
            {
                Name = dto.Name,
                Distance = dto.Distance,
                EntryFee = dto.EntryFee,
                EventId = eventId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category created successfully", categoryId = category.CategoryId });
        }

        [HttpPut("categories/{id}")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> UpdateCategory(int id, CreateUpdateCategoryDto dto)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            var category = await _context.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();
            if (category.Event?.OrganiserId != organiserId) return Forbid("You do not own this event.");

            category.Name = dto.Name;
            category.Distance = dto.Distance;
            category.EntryFee = dto.EntryFee;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Category updated successfully" });
        }

        [HttpDelete("categories/{id}")]
        [AuthorizeRole("Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId");
            var category = await _context.Categories.Include(c => c.Event).FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return NotFound();
            if (category.Event?.OrganiserId != organiserId) return Forbid("You do not own this event.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}