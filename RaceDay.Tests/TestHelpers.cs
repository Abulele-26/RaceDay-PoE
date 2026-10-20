using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using RaceDay.API.Data;
using System.Collections.Generic;
using System.Text;

namespace RaceDay.Tests
{
    public static class TestHelpers
    {
        public static RaceDayDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new RaceDayDbContext(options);
        }

        // Fixed ISession Mocking
        public static Mock<ISession> GetMockSession(int? userId, string? role)
        {
            var sessionStore = new Dictionary<string, byte[]>();
            if (userId.HasValue) sessionStore["UserId"] = BitConverter.GetBytes(userId.Value);
            if (role != null) sessionStore["UserRole"] = Encoding.UTF8.GetBytes(role);

            var mockSession = new Mock<ISession>();

            // THIS IS THE FIX FOR THE LOGIN TEST
            mockSession.SetupGet(s => s.IsAvailable).Returns(true);

            mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] value) =>
                {
                    if (sessionStore.TryGetValue(key, out var val))
                    {
                        value = val;
                        return true;
                    }
                    value = null!;
                    return false;
                });

            return mockSession;
        }

        public static T SetupControllerWithSession<T>(T controller, int? userId, string? role) where T : ControllerBase
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.Session = GetMockSession(userId, role).Object;

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = mockHttpContext
            };
            return controller;
        }
    }
}