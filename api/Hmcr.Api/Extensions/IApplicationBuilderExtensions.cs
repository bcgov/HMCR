using Hmcr.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;

namespace Hmcr.Api.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static void UseHmcrCors(this IApplicationBuilder app)
        {
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Content-Disposition"));
        }

        public static void UseExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        public static void UseHmcrEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public static void UseHmcrSpa(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }

        public static void UseHmcrSwagger(this IApplicationBuilder app, IWebHostEnvironment env, string url)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(url, "HMCR REST API v1");
                options.DocExpansion(DocExpansion.None);
            });
        }

        public static void UseHmcrHealthCheck(this IApplicationBuilder app)
        {
            var healthCheckOptions = new HealthCheckOptions
            {
                ResponseWriter = async (c, r) =>
                {
                    c.Response.ContentType = MediaTypeNames.Application.Json;
                    var result = JsonSerializer.Serialize(
                       new
                       {
                           checks = r.Entries.Select(e =>
                      new {
                          description = e.Key,
                          status = e.Value.Status.ToString(),
                          tags = e.Value.Tags,
                          responseTime = e.Value.Duration.TotalMilliseconds
                      }),
                           totalResponseTime = r.TotalDuration.TotalMilliseconds
                       });
                    await c.Response.WriteAsync(result);
                }
            };

            app.UseHealthChecks("/healthz", healthCheckOptions);
        }
    }
}
