using Hmcr.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

        public RequiresPermissionFilter(IAuthorizationService authService, PermissionRequirement requiredPermissions)
        {
            _authService = authService;
            _requiredPermissions = requiredPermissions;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var result = await _authService.AuthorizeAsync(context.HttpContext.User,
                context.ActionDescriptor.DisplayName,
                _requiredPermissions);

            if (!result.Succeeded)
            {
                context.Result = new UnauthorizedObjectResult(new Error
                {
                    ErrorCode = StatusCodes.Status403Forbidden,
                    Message = "Access Denied"
                });
            }
        }
    }
}
