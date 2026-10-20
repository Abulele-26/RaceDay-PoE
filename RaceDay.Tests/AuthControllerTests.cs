using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Controllers;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_ShouldReturnOk_WhenValidData()
        {
            // Arrange
            var context = TestHelpers.GetInMemoryDbContext();
            var controller = new AuthController(context);
            var dto = new RegisterDto { FullName = "John Doe", Email = "john@test.com", Password = "Password123!", Role = "Participant" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Single(context.Users);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailExists()
        {
            // Arrange
            var context = TestHelpers.GetInMemoryDbContext();
            context.Users.Add(new User { Email = "john@test.com", FullName = "Existing", PasswordHash = "hash", Role = "Participant" });
            await context.SaveChangesAsync();

            var controller = new AuthController(context);
            var dto = new RegisterDto { FullName = "John Doe", Email = "john@test.com", Password = "Password123!", Role = "Participant" };

            // Act
            var result = await controller.Register(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ShouldReturnOkAndSetSession_WhenValidCredentials()
        {
            // Arrange
            var context = TestHelpers.GetInMemoryDbContext();
            context.Users.Add(new User { Email = "john@test.com", FullName = "John", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), Role = "Organiser" });
            await context.SaveChangesAsync();

            var controller = TestHelpers.SetupControllerWithSession(new AuthController(context), null, null);
            var dto = new LoginDto { Email = "john@test.com", Password = "Password123!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.True(controller.HttpContext.Session.IsAvailable);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidPassword()
        {
            // Arrange
            var context = TestHelpers.GetInMemoryDbContext();
            context.Users.Add(new User { Email = "john@test.com", FullName = "John", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), Role = "Organiser" });
            await context.SaveChangesAsync();

            var controller = new AuthController(context);
            var dto = new LoginDto { Email = "john@test.com", Password = "WrongPassword!" };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}