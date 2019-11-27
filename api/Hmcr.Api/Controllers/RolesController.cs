using System.Collections.Generic;
using System.Threading.Tasks;
using Hmcr.Api.Authorization;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.Role;
using Microsoft.AspNetCore.Mvc;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private IRoleService _roleSvc;

        public RolesController(IRoleService roleSvc)
        {
            _roleSvc = roleSvc;
        }

        [HttpGet("")]
        [RequiresPermission(Permissions.RoleRead)]
        public async Task<ActionResult<IEnumerable<RoleSearchDto>>> GetRolesAsync([FromQuery]string searchText = null, [FromQuery]bool? isActive = true)
        {
            return Ok(await _roleSvc.GetRolesAync(searchText, isActive));
        }
    }
}