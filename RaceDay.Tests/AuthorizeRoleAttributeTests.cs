using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions; // ADDED THIS
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using RaceDay.API.Attributes;
using System.Collections.Generic;
using Xunit;

namespace RaceDay.Tests
{
    public class AuthorizeRoleAttributeTests
    {
        [Fact]
        public void OnAuthorization_ShouldReturnUnauthorized_WhenUserNotLoggedIn()
        {
            // Arrange
            var attribute = new AuthorizeRoleAttribute("Organiser");
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Session = TestHelpers.GetMockSession(null, null).Object; // No session

            var context = new AuthorizationFilterContext(
                // FIXED: Added ActionDescriptor
                new ActionContext
                {
                    HttpContext = mockHttpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor()
                },
                new List<IFilterMetadata>()
            );

            // Act
            attribute.OnAuthorization(context);

            // Assert
            Assert.IsType<UnauthorizedResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ShouldReturnForbidden_WhenUserHasWrongRole()
        {
            // Arrange
            var attribute = new AuthorizeRoleAttribute("Organiser");
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Session = TestHelpers.GetMockSession(1, "Participant").Object; // Participant

            var context = new AuthorizationFilterContext(
                // FIXED: Added ActionDescriptor
                new ActionContext
                {
                    HttpContext = mockHttpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor()
                },
                new List<IFilterMetadata>()
            );

            // Act
            attribute.OnAuthorization(context);

            // Assert
            Assert.IsType<ForbidResult>(context.Result);
        }

        [Fact]
        public void OnAuthorization_ShouldAllowAccess_WhenUserHasCorrectRole()
        {
            // Arrange
            var attribute = new AuthorizeRoleAttribute("Organiser");
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Session = TestHelpers.GetMockSession(1, "Organiser").Object; // Organiser

            var context = new AuthorizationFilterContext(
                // FIXED: Added ActionDescriptor
                new ActionContext
                {
                    HttpContext = mockHttpContext,
                    RouteData = new RouteData(),
                    ActionDescriptor = new ActionDescriptor()
                },
                new List<IFilterMetadata>()
            );

            // Act
            attribute.OnAuthorization(context);

            // Assert
            Assert.Null(context.Result);
        }
    }
}