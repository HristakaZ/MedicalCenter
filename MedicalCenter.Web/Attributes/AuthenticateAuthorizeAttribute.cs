using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace MedicalCenter.Web.Attributes
{
    public class AuthenticateAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[]? _roles;

        public AuthenticateAuthorizeAttribute(params string[]? roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            int sessionUserID = context.HttpContext.Session.GetInt32("UserID") ?? 0;
            bool isAuthenticated = sessionUserID != 0;
            // Check if the user is authenticated
            if (!isAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Users", null);
                return;
            }

            if (_roles != null && _roles.Any() && !_roles.Any(x => x.IsNullOrEmpty()))
            {
                string sessionUserRole = context.HttpContext.Session.GetString("UserRole")!;
                // Check if the user has the required role
                if (!_roles.Contains(sessionUserRole))
                {
                    context.Result = new RedirectToActionResult("Index", "Home", null);
                    return;
                }
            }
        }
    }
}
