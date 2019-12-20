using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Runtime.Versioning;
using Hmcr.Model;
using Microsoft.Extensions.Hosting;
using Hmcr.Api.Controllers.Base;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/version")]
    [ApiController]
    public class VersionController : HmcrControllerBase
    {
        private const string CommitKey = "OPENSHIFT_BUILD_COMMIT";

        private IConfiguration _config;
        private IWebHostEnvironment _env;
        
        public VersionController(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }

        [Route("")]
        [HttpGet]
        public ActionResult<VersionInfo> GetVersionInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var creationTime = System.IO.File.GetLastWriteTimeUtc(assembly.Location);
            var environment = "";

            if (_env.IsProduction())
            {
                environment = "Production";
            }
            else if (_env.IsStaging())
            {
                environment = "Test";
            }
            else if (_env.IsDevelopment() || _env.IsEnvironment("Local"))
            {
                environment = "Development";
            }
            else if (_env.IsEnvironment("Training"))
            {
                environment = "Training";
            }
            else if (_env.IsEnvironment("UAT"))
            {
                environment = "UAT";
            }

            var versionInfo = new VersionInfo()
            {
                Name = assembly.GetName().Name,
                Version = _config.GetSection("Constants:Version").Value,
                Description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description,
                Copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright,
                FileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version,
                FileCreationTime = creationTime.ToString("O"),
                InformationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
                TargetFramework = assembly.GetCustomAttribute<TargetFrameworkAttribute>().FrameworkName,
                ImageRuntimeVersion = assembly.ImageRuntimeVersion,
                Commit = _config[CommitKey],
                Environment = environment
            };

            return Ok(versionInfo);
        }
    }
}