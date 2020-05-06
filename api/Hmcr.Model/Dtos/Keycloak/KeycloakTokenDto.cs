using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.Keycloak
{
    public class KeycloakTokenDto
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }

}
