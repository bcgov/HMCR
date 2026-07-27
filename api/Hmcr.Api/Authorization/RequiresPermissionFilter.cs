using Hmcr.Api.Observability;
using Hmcr.Model;
using Hmcr.Model.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Api.Authorization
{
    public class RequiresPermissionFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationService _authService;
        private readonly PermissionRequirement _requiredPermissions;
        private readonly ILogger<RequiresPermissionFilter> _logger;

        public RequiresPermissionFilter(
            IAuthorizationService authService,
            PermissionRequirement requiredPermissions,
            ILogger<RequiresPermissionFilter> logger)
        {
            _authService = authService;
            _requiredPermissions = requiredPermissions;
            _logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var result = await _authService.AuthorizeAsync(context.HttpContext.User,
                context.ActionDescriptor.DisplayName,
                _requiredPermissions);

            if (!result.Succeeded)
            {
                var supportId = HmcrLogContext.CreateSupportId();
                var problem = new ValidationProblemDetails()
                {
                    Type = "https://hmcr.bc.gov.ca/exception",
                    Title = "Access denied",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Insufficient permission.",
                    Instance = context.HttpContext.Request.Path
                };

                using (_logger.BeginScope(HmcrLogContext.CreateHttpScope(
                    context.HttpContext,
                    null,
                    HmcrLogConstants.Sources.Api,
                    HmcrLogContext.GetOperation(context.HttpContext),
                    supportId,
                    null,
                    StatusCodes.Status401Unauthorized)))
                {
                    _logger.LogWarning("Authorization failed {SupportId}", supportId);
                }

                HmcrLogContext.EnrichProblemDetails(problem, context.HttpContext, supportId);

                context.Result = new UnauthorizedObjectResult(problem);
            }
        }
    }
}
