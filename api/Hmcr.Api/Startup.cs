using Hmcr.Api.Extensions;
using Hmcr.Chris;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


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
            services.AddHttpContextAccessor();
            services.AddHmcrAuthentication();
            services.AddHmcrDbContext(Configuration.GetConnectionString("Hmcr"));
            services.AddCors();
            services.AddHmcrControllers();
            services.AddHmcrAutoMapper();
            services.AddHmcrApiVersioning();
            services.AddHmcrTypes();
            services.AddHmcrSwagger(_env);
            services.AddChrisHttpClient(Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseExceptionMiddleware();
            app.UseAuthentication();
            app.UseHmcrCors();
            app.UseHmcrSwagger(env, Configuration.GetSection("Constants:SwaggerApiUrl").Value);
            app.UseRouting();
            app.UseAuthorization();
            app.UseHmcrEndpoints();
        }
    }
}
