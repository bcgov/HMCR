using System.Collections.Generic;
using System.Threading.Tasks;
using Hmcr.Domain.Services;
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
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetActiveRolesAsync()
        {
            return Ok(await _roleSvc.GetActiveRolesAsync());
        }
    }
}