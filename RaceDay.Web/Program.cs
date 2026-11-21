using Microsoft.AspNetCore.Authentication.Cookies;
using RaceDay.Web.Services;

namespace RaceDay.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // 1. Register HttpClient to talk to your Part 2 API
            builder.Services.AddHttpClient("RaceDayApi", client =>
            {
                // FIXED: Read from configuration (appsettings.json or Docker Environment Variable)
                // If not found, fallback to the actual port your API runs on (5291)
                var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5291/";
                client.BaseAddress = new Uri(baseUrl);
            });

            // 2. Add Session services
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // 3. Add Cookie Authentication (for login state)
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                });

            // 4. Register the ApiService (Crucial for making API calls)
            builder.Services.AddScoped<ApiService>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // IMPORTANT: Disabled for Docker to prevent HTTP->HTTPS redirect loops.
            // app.UseHttpsRedirection();

            // Note: .NET 10 templates use MapStaticAssets(), but UseStaticFiles() is 
            // perfectly fine and ensures standard static files (css/js) load correctly.
            app.UseStaticFiles();

            app.UseRouting();

            // IMPORTANT: The order of these three lines is non-negotiable!
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}