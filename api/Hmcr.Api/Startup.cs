using Hangfire;
using Hangfire.AspNetCore;
using Hmcr.Api.Authentication;
using Hmcr.Api.Extensions;
using Hmcr.Bceid;
using Hmcr.Chris;
using Hmcr.Domain.Hangfire;
using Hmcr.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Hmcr.Api
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetValue<string>("CONNECTION_STRING");

            services.AddHttpContextAccessor();
            services.AddHmcrAuthentication(Configuration);
            services.AddHmcrDbContext(connectionString);
            services.AddCors();
            services.AddHmcrControllers();
            services.AddHmcrAutoMapper();
            services.AddHmcrApiVersioning();
            services.AddHmcrTypes();
            services.AddHmcrSwagger(_env);
            services.AddChrisHttpClient(Configuration);
            services.AddBceidSoapClient(Configuration);
            services.AddHmcrHangfire(connectionString);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISubmissionObjectJobService jobService, 
            IServiceScopeFactory serviceScopeFactory, IServiceAreaService svcAreaService)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseHmcrCors();
            app.UseExceptionMiddleware();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseRouting();
            app.UseHmcrEndpoints();
            app.UseHmcrSwagger(env, Configuration.GetSection("Constants:SwaggerApiUrl").Value);
            app.UseHangfireDashboard();

            //Register Hangfire Recurring Jobs 
            var serviceAreas = svcAreaService.GetAllServiceAreas();
            SubmissionObjectJobService.RegisterReportingJobs(serviceAreas);
        }
    }
}
