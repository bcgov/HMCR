using Hmcr.Model.Dtos.RolePermission;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.Permission
{
    public class PermissionDto
    {
        public decimal PermissionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
