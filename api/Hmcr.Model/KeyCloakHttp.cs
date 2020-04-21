using System.Text.Json.Serialization;

namespace Hmcr.Model
{
    public class KeycloakTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }

    public class KeycloakUserResponse
    {
        [JsonPropertyName("id")]
        public string UserId { get; set; }
        public string Email { get; set; }
    }

    public class KeycloakUserPasswordResetRequest
    {
        public string Type => "password";
        public string Value { get; set; }
        public bool Temporary => false;

        public KeycloakUserPasswordResetRequest(string value)
        {
            Value = value;
        }
    }
}
