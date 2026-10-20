using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.API.Attributes
{
    // This attribute can be applied to controllers or individual endpoints
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check if the user is logged in (Session contains UserRole)
            var userRole = context.HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(userRole))
            {
                // 401 Unauthorized if not logged in
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check if the user has one of the allowed roles
            if (_roles.Length > 0 && !_roles.Contains(userRole))
            {
                // 403 Forbidden if logged in but wrong role
                context.Result = new ForbidResult();
            }
        }
    }
}