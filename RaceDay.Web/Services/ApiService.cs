using Microsoft.AspNetCore.Http;
using RaceDay.API.DTOs;
using RaceDay.Web.DTOs; // Added to find your copied DTOs
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
        // POST REQUESTS (File Uploads)
        // ==========================================

        public async Task<bool> CreateEventAsync(CreateUpdateEventDto eventDto, IFormFile? bannerImage)
        {
            AddAuthHeader();

            using var formContent = new MultipartFormDataContent();

            // Matching the properties from your CreateUpdateEventDto
            formContent.Add(new StringContent(eventDto.Name), "Name");
            formContent.Add(new StringContent(eventDto.Description ?? ""), "Description");
            formContent.Add(new StringContent(eventDto.Date.ToString("yyyy-MM-ddTHH:mm:ss")), "Date");
            formContent.Add(new StringContent(eventDto.Location ?? ""), "Location");
            formContent.Add(new StringContent(eventDto.Distance.ToString()), "Distance");
            formContent.Add(new StringContent(eventDto.EventType ?? ""), "EventType");

            if (bannerImage != null && bannerImage.Length > 0)
            {
                using var fileStream = bannerImage.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(bannerImage.ContentType);

                // Must match the parameter in your API Controller (e.g., [FromForm] IFormFile BannerImage)
                formContent.Add(fileContent, "BannerImage", bannerImage.FileName);
            }

            var response = await _httpClient.PostAsync("api/events", formContent);
            return response.IsSuccessStatusCode;
        }
    }
}