using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.UserRole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.Role
{
    public class RoleDto
    {
        public RoleDto()
        {
            RolePermissions = new HashSet<RolePermissionDto>();
            UserRoles = new HashSet<UserRoleDto>();
        }

        public decimal RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<RolePermissionDto> RolePermissions { get; set; }
        public virtual ICollection<UserRoleDto> UserRoles { get; set; }
    }
}
