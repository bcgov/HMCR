using System;
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

        [HttpGet("{id}", Name = "GetRole")]
        [RequiresPermission(Permissions.RoleRead)]
        public async Task<ActionResult<RoleDto>> GetRoleAsync(decimal id)
        {
            return await _roleSvc.GetRoleAsync(id);
        }

        [HttpPost]
        [RequiresPermission(Permissions.RoleWrite)]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleCreateDto role)
        {
            var response = await _roleSvc.CreateRoleAsync(role);

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return CreatedAtRoute("GetRole", new { id = response.RoleId }, await _roleSvc.GetRoleAsync(response.RoleId));
        }

        [HttpPut("{id}")]
        [RequiresPermission(Permissions.RoleWrite)]
        public async Task<ActionResult> UpdateRole(decimal id, RoleUpdateDto role)
        {
            if (id != role.RoleId)
            {
                throw new Exception($"The system role ID from the query string does not match that of the body.");
            }

            var response = await _roleSvc.UpdateRoleAsync(role);

            if (response.NotFound)
            {
                return NotFound();
            }

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequiresPermission(Permissions.RoleWrite)]
        public async Task<ActionResult> DeleteRole(decimal id, RoleDeleteDto role)
        {
            if (id != role.RoleId)
            {
                throw new Exception($"The system role ID from the query string does not match that of the body.");
            }

            var response = await _roleSvc.DeleteRoleAsync(role);

            if (response.NotFound)
            {
                return NotFound();
            }

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return NoContent();
        }
    }
}