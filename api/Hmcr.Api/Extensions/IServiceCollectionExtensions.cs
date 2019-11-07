using AutoMapper;
using Hmcr.Api.Authentication;
using Hmcr.Api.Authorization;
using Hmcr.Data.Database;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Mappings;
using Hmcr.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;
using Swashbuckle.AspNetCore.Swagger;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Hmcr.Model.JsonConverters;

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

        public static void AddHmcrMvc(this IServiceCollection services)
        {
            services
                .AddControllers(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.Converters.Add(new LongToStringConverter()); 
                    options.JsonSerializerOptions.Converters.Add(new IntToStringConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public static void AddHmcrDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));
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

        public static void AddHmcrAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = SmAuthenticationOptions.Scheme;
                options.DefaultChallengeScheme = SmAuthenticationOptions.Scheme;
            })
            .AddScheme<SmAuthenticationOptions, SmAuthenticationHandler>(SmAuthenticationOptions.Scheme, null);
        }

        public static void AddHmcrSwagger(this IServiceCollection services, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "HMCR REST API",
                        Description = "Highway Maintenance Contract Reporting System"
                    });
                });
            }
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
            services.AddScoped<SmHeaders, SmHeaders>();

            //Permission Handler
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
        }
    }
}
