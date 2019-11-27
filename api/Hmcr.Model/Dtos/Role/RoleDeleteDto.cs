using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.Role
{
    public class RoleDeleteDto
    {
        [JsonPropertyName("id")]
        public decimal RoleId { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
