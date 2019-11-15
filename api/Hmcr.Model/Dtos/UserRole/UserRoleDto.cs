using Hmcr.Model.Dtos.Role;
using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.UserRole
{
    public class UserRoleDto
    {
        public decimal UserRoleId { get; set; }
        public decimal RoleId { get; set; }
        public decimal SystemUserId { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
