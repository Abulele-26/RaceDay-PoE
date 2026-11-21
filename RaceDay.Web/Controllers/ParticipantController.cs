using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;
using System.Security.Claims;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Participant")]
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private int GetUserId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out int id) ? id : 0;
        }

        public async Task<IActionResult> EventDetails(int id)
        {
            var ev = await _apiService.GetEventByIdAsync(id);
            if (ev == null) return NotFound();

            // Fetch categories for this event
            var categories = await _apiService.GetCategoriesForEventAsync(id);
            ViewBag.Categories = categories;

            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int eventId, int categoryId)
        {
            if (categoryId == 0)
            {
                TempData["ErrorMessage"] = "Please select a category.";
                return RedirectToAction(nameof(EventDetails), new { id = eventId });
            }

            var success = await _apiService.EnrollAsync(eventId, categoryId);
            if (success) TempData["SuccessMessage"] = "Successfully enrolled!";
            else TempData["ErrorMessage"] = "Failed to enrol. You might already be enrolled.";

            return RedirectToAction(nameof(EventDetails), new { id = eventId });
        }

        public async Task<IActionResult> MyEvents()
        {
            var userId = GetUserId();
            var enrollments = await _apiService.GetMyEnrollmentsAsync(userId);
            return View(enrollments);
        }

        public async Task<IActionResult> MyResults()
        {
            var userId = GetUserId();
            var results = await _apiService.GetMyResultsAsync(userId);
            return View(results);
        }

        public async Task<IActionResult> Profile()
        {
            var userId = GetUserId();
            var profile = await _apiService.GetProfileAsync(userId);
            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UpdateUserProfileDto model, IFormFile? ProfilePicture)
        {
            if (!ModelState.IsValid) return View("Profile", model);

            var userId = GetUserId();
            var success = await _apiService.UpdateProfileAsync(userId, model, ProfilePicture);

            if (success) TempData["SuccessMessage"] = "Profile updated successfully!";
            else TempData["ErrorMessage"] = "Failed to update profile.";

            return RedirectToAction(nameof(Profile));
        }
    }
}