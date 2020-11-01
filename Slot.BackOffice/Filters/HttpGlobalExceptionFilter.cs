using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Slot.Model;
using System;

namespace Slot.BackOffice.Filters
{
    public sealed class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var isDevelopment = env.IsDevelopment();
            logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

            if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new Data.Responses.ForbiddenRequestResult(
                                                        isDevelopment ? context.Exception.Message : string.Empty,
                                                        ErrorCode.AccessDenied);
            }
            else
            {
                context.Result = new Data.Responses.BadRequestResult(
                                                        isDevelopment ? context.Exception.Message : string.Empty,
                                                        ErrorCode.InternalError);
            }

            context.ExceptionHandled = !isDevelopment;
        }
    }
}
