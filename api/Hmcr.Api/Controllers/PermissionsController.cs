using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.Permission;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/permissions")]
    [ApiController]
    public class PermissionsController : HmcrControllerBase
    {
        private IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("")]
        [RequiresPermission(Permissions.RoleRead)]
        public async Task<ActionResult<IEnumerable<PermissionDto>>> GetActivePermissionsAsync()
        {
            return Ok(await _permissionService.GetActivePermissionsAsync());
        }

    }
}
