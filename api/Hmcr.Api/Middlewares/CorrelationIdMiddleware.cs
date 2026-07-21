using Hmcr.Api.Observability;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Hmcr.Api.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            HmcrLogContext.EnsureCorrelationId(context);
            HmcrLogContext.SetResponseHeaders(context);

            await _next(context);
        }
    }
}
