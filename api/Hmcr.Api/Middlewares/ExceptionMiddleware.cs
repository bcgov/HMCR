using Hmcr.Api.Extensions;
using Hmcr.Api.Observability;
using Hmcr.Model;
using Hmcr.Model.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Hmcr.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, HmcrCurrentUser currentUser)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (httpContext.Response.HasStarted || httpContext.RequestAborted.IsCancellationRequested)
                    return;

                var supportId = HmcrLogContext.CreateSupportId();
                using (_logger.BeginScope(HmcrLogContext.CreateHttpScope(
                    httpContext,
                    currentUser,
                    HmcrLogConstants.Sources.Api,
                    HmcrLogContext.GetOperation(httpContext),
                    supportId,
                    HmcrLogConstants.ErrorCodes.ApiUnexpected,
                    StatusCodes.Status500InternalServerError)))
                {
                    _logger.LogError(ex, "Unhandled API exception {SupportId}", supportId);
                }

                await HandleExceptionAsync(httpContext, supportId);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, string supportId)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            var problem = HmcrLogContext.CreateUnexpectedProblem(
                context,
                supportId,
                HmcrLogConstants.ErrorCodes.ApiUnexpected);

            await context.Response.WriteJsonAsync(problem, "application/problem+json");
        }
    }
}
