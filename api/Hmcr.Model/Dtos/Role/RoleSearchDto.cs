using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.Role
{
    public class RoleSearchDto
    {
        [JsonPropertyName("id")]
        public decimal RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
        public string IsInternal { get; set; }
        public bool IsActive => EndDate == null || EndDate > DateTime.Today;
    }
}
