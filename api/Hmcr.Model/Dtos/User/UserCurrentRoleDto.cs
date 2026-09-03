using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserCurrentRoleDto
    {
        [JsonPropertyName("id")]
        public decimal RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
