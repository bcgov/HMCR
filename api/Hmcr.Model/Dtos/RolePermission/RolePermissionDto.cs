using Hmcr.Model.Dtos.Permission;
using Hmcr.Model.Dtos.Role;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.RolePermission
{
    public class RolePermissionDto
    {
        public decimal RolePermissionId { get; set; }
        public decimal RoleId { get; set; }
        public decimal PermissionId { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
