using Hmcr.Api.Observability;
using Hmcr.Model;
using Hmcr.Model.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hmcr.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, HmcrCurrentUser currentUser)
        {
            if (ShouldSkip(context))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var scope = HmcrLogContext.CreateHttpScope(
                context,
                currentUser,
                HmcrLogConstants.Sources.Api,
                HmcrLogContext.GetOperation(context));

            using (_logger.BeginScope(scope))
            {
                try
                {
                    await _next(context);
                    stopwatch.Stop();

                    scope["statusCode"] = context.Response.StatusCode;
                    scope["elapsedMs"] = stopwatch.ElapsedMilliseconds;

                    _logger.LogInformation(
                        "HTTP {HttpMethod} {RequestPath} responded {StatusCode} in {ElapsedMs} ms",
                        context.Request.Method,
                        context.Request.Path.Value,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);
                }
                catch (Exception)
                {
                    stopwatch.Stop();

                    scope["statusCode"] = StatusCodes.Status500InternalServerError;
                    scope["elapsedMs"] = stopwatch.ElapsedMilliseconds;

                    _logger.LogError(
                        "HTTP {HttpMethod} {RequestPath} failed in {ElapsedMs} ms",
                        context.Request.Method,
                        context.Request.Path.Value,
                        stopwatch.ElapsedMilliseconds);
                    throw;
                }
            }
        }

        private static bool ShouldSkip(HttpContext context)
        {
            return context.Request.Path.StartsWithSegments("/healthz");
        }
    }
}
