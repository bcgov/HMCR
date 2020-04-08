using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Hmcr.Api.Authentication;
using Hmcr.Api.Authorization;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Mappings;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.JsonConverters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NetCore.AutoRegisterDi;
using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Hmcr.Api.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddHmcrApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("version");
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });
        }

        public static void AddHmcrControllers(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .ConfigureApiBehaviorOptions(setupAction =>
                {
                    setupAction.InvalidModelStateResponseFactory = ValidationUtils.GetValidationErrorResult;
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new LongToStringConverter());
                    options.JsonSerializerOptions.Converters.Add(new IntToStringConverter());
                })
                //.AddNewtonsoftJson(options =>
                //{
                //    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //    options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                //    options.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
                //    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                //})
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public static void AddHmcrDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));
        }

        public static void AddHmcrAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new EntityToModelProfile());
                cfg.AddProfile(new ModelToEntityProfile());
            });

            var mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        public static void AddHmcrAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = config.GetValue<string>("JWT:Authority");
                options.Audience = config.GetValue<string>("JWT:Audience");
                options.RequireHttpsMetadata = false;
                options.IncludeErrorDetails = true;
                options.EventsType = typeof(HmcrJwtBearerEvents);
            });
        }

        public static void AddHmcrSwagger(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "HMCR REST API",
                    Description = "Highway Maintenance Contract Reporting System"
                });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                options.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(securitySchema, new[] { "Bearer" });
                options.AddSecurityRequirement(securityRequirement);
            });
        }

        public static void AddHmcrTypes(this IServiceCollection services)
        {
            var assemblies = Assembly.GetExecutingAssembly()
                .GetReferencedAssemblies()
                .Where(a => a.FullName.StartsWith("Hmcr"))
                .Select(Assembly.Load).ToArray();

            //Services
            services.RegisterAssemblyPublicNonGenericClasses(assemblies)
                 .Where(c => c.Name.EndsWith("Service"))
                 .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            //Repository
            services.RegisterAssemblyPublicNonGenericClasses(assemblies)
                 .Where(c => c.Name.EndsWith("Repository"))
                 .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

            //Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //SmHeaders
            services.AddScoped<HmcrCurrentUser, HmcrCurrentUser>();

            //Permission Handler
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            //FieldValidationService as Singleton
            services.AddSingleton<IFieldValidatorService, FieldValidatorService>();

            //RegexDefs as Singleton
            services.AddSingleton<RegexDefs>();

            //Jwt Bearer Handler
            services.AddScoped<HmcrJwtBearerEvents>();

            services.AddSingleton<EmailBody>();
        }

        public static void AddHmcrHangfire(this IServiceCollection services, string connectionString, bool runServer, int workerCount)
        {
            services.AddHangfire(configuration => configuration
                .UseSerilogLogProvider()
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            if (runServer)
            {
                services.AddHangfireServer(options =>
                {
                    options.WorkerCount = workerCount;
                });
            }
        }

        public static void AddHmcrHealthCheck(this IServiceCollection services, string connectionString)
        {
            services.AddHealthChecks()
                .AddSqlServer(connectionString, name: "HMCR-DB-Check", failureStatus: HealthStatus.Degraded, tags: new string[] { "sql", "db" });
        }
    }
}
