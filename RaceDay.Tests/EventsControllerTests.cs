using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Controllers;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using Xunit;

namespace RaceDay.Tests
{
    public class EventsControllerTests
    {
        [Fact]
        public async Task CreateEvent_ShouldSucceed_WhenUserIsOrganiser()
        {
            // Arrange
            var context = TestHelpers.GetInMemoryDbContext();
            // Mock user as Organiser (UserId = 1)
            var controller = TestHelpers.SetupControllerWithSession(new EventsController(context), 1, "Organiser");

            var dto = new CreateUpdateEventDto
            {
                Name = "Cape Town Marathon",
                Description = "Annual marathon",
                EventDate = DateTime.UtcNow.AddDays(30),
                Location = "Cape Town",
                Distance = 42.2,
                EventType = "Run"
            };

            // Act
            var result = await controller.CreateEvent(dto);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.Single(context.Events);
        }

        [Fact]
        public async Task EnrollInEvent_ShouldSucceed_WhenParticipantEnrolls()
        {
            // Arrange
            var context = TestHelpers.GetInMemoryDbContext();
            // Setup an Event and Category
            var ev = new Event { Name = "Test", EventDate = DateTime.UtcNow, Location = "Test", OrganiserId = 99 };
            context.Events.Add(ev);
            var cat = new Category { Name = "10km", Distance = 10, EntryFee = 100, EventId = ev.EventId };
            context.Categories.Add(cat);
            await context.SaveChangesAsync();

            // Mock user as Participant (UserId = 2)
            var controller = TestHelpers.SetupControllerWithSession(new EnrollmentsController(context), 2, "Participant");
            var dto = new CreateEnrollmentDto { CategoryId = cat.CategoryId };

            // Act
            var result = await controller.EnrollInEvent(ev.EventId, dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Single(context.Enrollments);
        }
    }
}