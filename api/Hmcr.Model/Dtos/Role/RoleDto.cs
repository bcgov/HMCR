using Hmcr.Model.Dtos.RolePermission;
using Hmcr.Model.Dtos.UserRole;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.Role
{
    public class RoleDto
    {
        public RoleDto()
        {
            Permissions = new List<decimal>();
        }
        [JsonPropertyName("id")]
        public decimal RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
        public string IsInternal { get; set; }
        public IList<decimal> Permissions { get; set; }
    }
}
