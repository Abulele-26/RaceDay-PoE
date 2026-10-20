using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Attributes;
using RaceDay.API.Data;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [AuthorizeRole("Organiser", "Participant")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return Ok(new UserProfileDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            });
        }

        [HttpPut("me")]
        [AuthorizeRole("Organiser", "Participant")]
        public async Task<IActionResult> UpdateMyProfile(UpdateUserProfileDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Check if new email is already taken by someone else
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != userId))
                return BadRequest("Email is already in use.");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Profile updated successfully" });
        }
    }
}