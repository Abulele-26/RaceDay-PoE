using Microsoft.AspNetCore.Http;
using RaceDay.Web.DTOs;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace RaceDay.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("RaceDayApi");
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // ==========================================
        // GET REQUESTS
        // ==========================================

        public async Task<List<EventDto>> GetEventsAsync()
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync("api/events");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EventDto>>() ?? new List<EventDto>();
            }
            return new List<EventDto>();
        }

        public async Task<EventDto?> GetEventByIdAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/events/{id}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<EventDto>();
            return null;
        }

        public async Task<List<CategoryDto>> GetCategoriesForEventAsync(int eventId)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/categories/event/{eventId}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<CategoryDto>>() ?? new();
            return new();
        }

        public async Task<List<EnrollmentDto>> GetEnrolmentsForEventAsync(int eventId)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/enrollments/event/{eventId}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<EnrollmentDto>>() ?? new();
            return new();
        }

        // ==========================================
        // POST REQUESTS (Auth)
        // ==========================================

        public async Task<HttpResponseMessage> LoginAsync(LoginDto loginDto)
        {
            return await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
        }

        public async Task<HttpResponseMessage> RegisterAsync(RegisterDto registerDto)
        {
            return await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
        }

        // ==========================================
        // POST & PUT REQUESTS (Events & File Uploads)
        // ==========================================

        public async Task<bool> CreateEventAsync(CreateUpdateEventDto eventDto, IFormFile? bannerImage)
        {
            AddAuthHeader();
            using var formContent = new MultipartFormDataContent();

            formContent.Add(new StringContent(eventDto.Name), "Name");
            formContent.Add(new StringContent(eventDto.Description ?? ""), "Description");
            formContent.Add(new StringContent(eventDto.EventDate.ToString("yyyy-MM-ddTHH:mm:ss")), "EventDate");
            formContent.Add(new StringContent(eventDto.Location ?? ""), "Location");
            formContent.Add(new StringContent(eventDto.Distance.ToString()), "Distance");
            formContent.Add(new StringContent(eventDto.EventType ?? ""), "EventType");

            if (bannerImage != null && bannerImage.Length > 0)
            {
                using var fileStream = bannerImage.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(bannerImage.ContentType);
                formContent.Add(fileContent, "BannerImage", bannerImage.FileName);
            }

            var response = await _httpClient.PostAsync("api/events", formContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateEventAsync(int id, CreateUpdateEventDto eventDto, IFormFile? bannerImage)
        {
            AddAuthHeader();
            using var formContent = new MultipartFormDataContent();

            formContent.Add(new StringContent(eventDto.Name), "Name");
            formContent.Add(new StringContent(eventDto.Description ?? ""), "Description");
            formContent.Add(new StringContent(eventDto.EventDate.ToString("yyyy-MM-ddTHH:mm:ss")), "EventDate");
            formContent.Add(new StringContent(eventDto.Location ?? ""), "Location");
            formContent.Add(new StringContent(eventDto.Distance.ToString()), "Distance");
            formContent.Add(new StringContent(eventDto.EventType ?? ""), "EventType");

            if (bannerImage != null && bannerImage.Length > 0)
            {
                using var fileStream = bannerImage.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(bannerImage.ContentType);
                formContent.Add(fileContent, "BannerImage", bannerImage.FileName);
            }

            var response = await _httpClient.PutAsync($"api/events/{id}", formContent);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            AddAuthHeader();
            var response = await _httpClient.DeleteAsync($"api/events/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateCategoryAsync(int eventId, CreateUpdateCategoryDto categoryDto)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync($"api/categories/event/{eventId}", categoryDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CreateResultAsync(CreateResultDto resultDto)
        {
            AddAuthHeader();
            var response = await _httpClient.PostAsJsonAsync("api/results", resultDto);
            return response.IsSuccessStatusCode;
        }

        // ==========================================
        // PARTICIPANT SPECIFIC METHODS (ADDED FOR STEP 5)
        // ==========================================

        public async Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(int userId)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/enrollments/user/{userId}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<EnrollmentDto>>() ?? new();
            return new();
        }

        public async Task<List<ResultDto>> GetMyResultsAsync(int userId)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/results/user/{userId}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<ResultDto>>() ?? new();
            return new();
        }

        public async Task<bool> EnrollAsync(int eventId, int categoryId)
        {
            AddAuthHeader();
            var enrollDto = new { EventId = eventId, CategoryId = categoryId };
            var response = await _httpClient.PostAsJsonAsync("api/enrollments", enrollDto);
            return response.IsSuccessStatusCode;
        }

        public async Task<UserProfileDto?> GetProfileAsync(int userId)
        {
            AddAuthHeader();
            var response = await _httpClient.GetAsync($"api/users/{userId}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<UserProfileDto>();
            return null;
        }

        public async Task<bool> UpdateProfileAsync(int userId, UpdateUserProfileDto profileDto, IFormFile? profilePicture)
        {
            AddAuthHeader();
            using var formContent = new MultipartFormDataContent();

            formContent.Add(new StringContent(profileDto.FullName ?? ""), "FullName");
            formContent.Add(new StringContent(profileDto.Email ?? ""), "Email");

            if (profilePicture != null && profilePicture.Length > 0)
            {
                using var fileStream = profilePicture.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(profilePicture.ContentType);
                formContent.Add(fileContent, "ProfilePicture", profilePicture.FileName);
            }

            var response = await _httpClient.PutAsync($"api/users/{userId}", formContent);
            return response.IsSuccessStatusCode;
        }
    }
}