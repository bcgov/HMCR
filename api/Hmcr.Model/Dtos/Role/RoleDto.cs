using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.UserRole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.Role
{
    public class RoleDto
    {
        public decimal RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
