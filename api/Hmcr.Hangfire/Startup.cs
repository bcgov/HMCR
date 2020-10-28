using Hangfire;
using Hmcr.Api.Extensions;
using Hmcr.Bceid;
using Hmcr.Chris;
using Hmcr.Data.Repositories;
using Hmcr.Domain.Hangfire;
using Hmcr.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace Hmcr.Hangfire
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
            var connectionString = Configuration.GetValue<string>("ConnectionStrings:HMCR");
            var runHangfireServer = Configuration.GetValue<bool>("Hangfire:EnableServer");
            var workerCount = Configuration.GetValue<int>("Hangfire:WorkerCount");

            services.AddHttpContextAccessor();
            services.AddHmcrDbContext(connectionString);
            services.AddHmcrAutoMapper();
            services.AddHmcrTypes();
            services.AddChrisHttpClient(Configuration);
            services.AddBceidSoapClient(Configuration);
            services.AddHmcrHangfire(connectionString, runHangfireServer, workerCount);
            services.AddHealthChecks().AddTypeActivatedCheck<HangfireHealthCheck>("Hangfire", HealthStatus.Unhealthy, connectionString);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISubmissionObjectJobService jobService,
            IServiceScopeFactory serviceScopeFactory, IServiceAreaService svcAreaService, ICodeLookupRepository codeLookupRepo, 
            IActivityRuleRepository activityRuleRepo, IFieldValidatorService validator)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionMiddleware();
            app.UseHmcrHealthCheck();
            app.UseHangfireDashboard();

            //Register Hangfire Recurring Jobs 
            var serviceAreas = svcAreaService.GetAllServiceAreas();

            var minutes = Configuration.GetValue<int>("Hangfire:ReportJobIntervalInMinutes");
            minutes = minutes < 1 ? 5 : minutes;

            foreach (var serviceArea in serviceAreas)
            {
                RecurringJob.AddOrUpdate<SubmissionObjectJobService>($"SA{serviceArea.ServiceAreaNumber}", x => x.RunReportingJob(serviceArea.ServiceAreaNumber), $"*/{minutes} * * * *");
            }

            //Inject Code Lookup
            validator.CodeLookup = codeLookupRepo.LoadCodeLookupCache();
            validator.ActivityCodeRuleLookup = activityRuleRepo.LoadActivityCodeRuleCache();

            minutes = Configuration.GetValue<int>("Hangfire:EmailJobIntervalInMinutes");
            minutes = minutes < 1 ? 30 : minutes;

            RecurringJob.AddOrUpdate<EmailJobService>("ResendEmails", x => x.ResendEmails(), $"*/{minutes} * * * *");
        }
    }
}
