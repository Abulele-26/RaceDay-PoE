using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.DTOs;
using RaceDay.Web.Services;
using System.Security.Claims;

namespace RaceDay.Web.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out int id) ? id : 0;
        }

        // 1. DASHBOARD
        
        public async Task<IActionResult> Dashboard()
        {
            var events = await _apiService.GetEventsAsync();
            ViewBag.TotalEvents = events.Count;
            return View(events);
        }

      
        // 2. EVENT MANAGEMENT (CRUD)
        
        public async Task<IActionResult> Events()
        {
            var events = await _apiService.GetEventsAsync();
            return View(events);
        }

        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(CreateUpdateEventDto model, IFormFile? BannerImage)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _apiService.CreateEventAsync(model, BannerImage);

            if (success)
            {
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Events));
            }

            ModelState.AddModelError(string.Empty, "Failed to create event. Please check your data.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var eventDto = await _apiService.GetEventByIdAsync(id);
            if (eventDto == null) return NotFound();

            var updateDto = new CreateUpdateEventDto
            {
                Name = eventDto.Name,
                Description = eventDto.Description,
                EventDate = eventDto.EventDate, 
                Location = eventDto.Location,
                Distance = eventDto.Distance,
                EventType = eventDto.EventType
            };

            ViewBag.EventId = id; // IMPORTANT: This passes the ID to the View

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, CreateUpdateEventDto model, IFormFile? BannerImage)
        {
            if (!ModelState.IsValid) return View(model);

            var success = await _apiService.UpdateEventAsync(id, model, BannerImage);

            if (success)
            {
                TempData["SuccessMessage"] = "Event updated successfully!";
                return RedirectToAction(nameof(Events));
            }

            ModelState.AddModelError(string.Empty, "Failed to update event.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var success = await _apiService.DeleteEventAsync(id);

            if (success) TempData["SuccessMessage"] = "Event deleted.";
            else TempData["ErrorMessage"] = "Could not delete event.";

            return RedirectToAction(nameof(Events));
        }

        // 3. CATEGORY MANAGEMENT
        
        public async Task<IActionResult> Categories(int eventId)
        {
            ViewBag.EventId = eventId;
            var categories = await _apiService.GetCategoriesForEventAsync(eventId);
            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCategory(int eventId, CreateUpdateCategoryDto model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Categories), new { eventId });

            var success = await _apiService.CreateCategoryAsync(eventId, model);

            if (success) TempData["SuccessMessage"] = "Category added!";
            else TempData["ErrorMessage"] = "Failed to add category.";

            return RedirectToAction(nameof(Categories), new { eventId });
        }

        // ==========================================
        // 4. VIEW ENROLMENTS
        // ==========================================
        public async Task<IActionResult> Enrolments(int eventId)
        {
            ViewBag.EventId = eventId;
            var enrolments = await _apiService.GetEnrolmentsForEventAsync(eventId);
            return View(enrolments);
        }

        // 5. RESULTS ENTRY
        
        public async Task<IActionResult> Results(int eventId)
        {
            ViewBag.EventId = eventId;
            var enrolments = await _apiService.GetEnrolmentsForEventAsync(eventId);
            return View(enrolments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitResult(int eventId, CreateResultDto model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Results), new { eventId });

            var success = await _apiService.CreateResultAsync(model);

            if (success) TempData["SuccessMessage"] = "Result published successfully!";
            else TempData["ErrorMessage"] = "Failed to publish result.";

            return RedirectToAction(nameof(Results), new { eventId });
        }
    }
}