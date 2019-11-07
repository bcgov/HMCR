using Hmcr.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Threading.Tasks;

namespace Hmcr.Api.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private ILogger _logger;
        private IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(ILogger logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var user = context.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            foreach (var permission in requirement.RequiredPermissions)
            {
                if (!user.HasClaim(HmcrClaimTypes.Permission, permission))
                {
                    _logger.Information("RequiresPermission - {user} - {url} - {permission}", user.Identity.Name, _httpContextAccessor.HttpContext.Request.Path, permission);

                    context.Fail();
                    return Task.CompletedTask;
                }
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
