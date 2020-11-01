using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Slot.BackOffice.Data.Authentication;
using System;
using System.Security.Claims;

namespace Slot.BackOffice.Filters
{
    /// <summary>
    /// Checks if the user is authorized to access the resource using strongly typed back office roles as parameters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class BackOfficeAuthorizeAttribute : TypeFilterAttribute
    {
        public BackOfficeAuthorizeAttribute(params Roles[] roles) : base(typeof(BackOfficeAuthorizeAttributeImplementation))
        {
            Arguments = new[] { roles };
        }

        private class BackOfficeAuthorizeAttributeImplementation : IAuthorizationFilter
        {
            private readonly Roles[] roles;

            public BackOfficeAuthorizeAttributeImplementation(params Roles[] roles)
            {
                this.roles = roles;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                var user = context.HttpContext.User;

                if (user.Identity.IsAuthenticated && !IsInRole(user))
                {
                    context.Result = new Data.Responses.ForbiddenRequestResult(string.Empty, Model.ErrorCode.AccessDenied);
                }
            }

            private bool IsInRole(ClaimsPrincipal user)
            {
                var isInRole = false || roles.Length == 0;

                foreach (var role in roles)
                {
                    if (user.IsInRole(role.ToString()))
                    {
                        isInRole = true;
                        break;
                    }
                }

                return isInRole;
            }
        }
    }
}
