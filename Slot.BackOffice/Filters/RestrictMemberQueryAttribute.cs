using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Slot.BackOffice.Filters
{
    /// <summary>
    /// Validates the operator the current user could query.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class RestrictMemberQueryAttribute : TypeFilterAttribute
    {
        public RestrictMemberQueryAttribute() : base(typeof(RestrictMemberQueryAttributeImplementation)) { }

        private class RestrictMemberQueryAttributeImplementation : IAsyncActionFilter
        {
            private readonly Roles[] restrictedRoles = new[]
            {
                Roles.CustomerService,
                Roles.CustomerServiceLimited,
            };

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                foreach (var arg in context.ActionArguments)
                {
                    if (arg.Value is IMemberQuery memberQuery)
                    {
                        if (IsMemberQueryRestricted(context.HttpContext.User, memberQuery))
                        {
                            context.Result = new Data.Responses.ForbiddenRequestResult(string.Empty, Model.ErrorCode.AccessDenied);

                            return;
                        }
                    }
                }

                await next();
            }

            private bool IsMemberQueryRestricted(ClaimsPrincipal user, IMemberQuery query)
            {
                var isMemberQueryRestricted = false;

                foreach (var role in restrictedRoles)
                {
                    if (user.IsInRole(role.ToString()) && string.IsNullOrWhiteSpace(query.MemberName))
                    {
                        isMemberQueryRestricted = true;
                        break;
                    }
                }

                return isMemberQueryRestricted;
            }
        }
    }
}