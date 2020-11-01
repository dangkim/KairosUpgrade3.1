using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Slot.BackOffice.Data.Authentication;
using Slot.BackOffice.Data.Queries;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Slot.BackOffice.Filters
{
    /// <summary>
    /// Validates the operator the current user could query.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public sealed class ValidateQueryOperatorAttribute : TypeFilterAttribute
    {
        public ValidateQueryOperatorAttribute() : base(typeof(ValidateQueryOperatorAttributeImplementation)) { }

        private class ValidateQueryOperatorAttributeImplementation : IAsyncActionFilter
        {
            private readonly Roles[] allowedRoles = new[]
            {
                Roles.Administrator,
                Roles.Compliance,
                Roles.GameAnalysis,
                Roles.GameAnalysisManager
            };

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                foreach (var arg in context.ActionArguments)
                {
                    if (arg.Value is IOperatorQuery query)
                    {
                        if (!IsAuthorized(context, query))
                        {
                            context.Result = new Data.Responses.BadRequestResult(string.Empty, Model.ErrorCode.WrongParameter);
                            return;
                        }
                    }
                }

                await next();
            }

            private bool IsAuthorized(ActionExecutingContext context, IOperatorQuery query)
            {
                var isAuthorized = true;
                var user = new JwtUser(context.HttpContext.User);

                if (!CanAccessAllOperators(context.HttpContext.User))
                {
                    var isOperatorIdModified = query.OperatorId.HasValue && user.OperatorId != query.OperatorId;
                    var isOperatorTagModified = !string.IsNullOrWhiteSpace(query.OperatorTag) && string.Compare(user.Operator, query.OperatorTag, true) != 0;
                    var isMultipleOperatorParamsModified = query.OperatorIds != null && (!query.OperatorIds.Contains(user.OperatorId) || query.OperatorIds.Length > 1);

                    var isIllegalQuery = isOperatorIdModified || isOperatorTagModified || isMultipleOperatorParamsModified;

                    if (isIllegalQuery)
                    {
                        isAuthorized = false;
                    }
                    else
                    {
                        query.OperatorId = user.OperatorId;
                        query.OperatorTag = user.Operator;
                        isAuthorized = true;
                    }
                }

                return isAuthorized;
            }

            private bool CanAccessAllOperators(ClaimsPrincipal user)
            {
                var canAccessAllOperators = false;

                foreach (var role in allowedRoles)
                {
                    if (user.IsInRole(role.ToString()))
                    {
                        canAccessAllOperators = true;
                        break;
                    }
                }

                return canAccessAllOperators;
            }
        }
    }
}
