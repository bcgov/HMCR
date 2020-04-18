using System.Text.Json.Serialization;

namespace Hmcr.Model
{
    public class KeyCloakTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }

    public class KeyCloakUserResponse
    {
        [JsonPropertyName("id")]
        public string UserId { get; set; }
        public string Email { get; set; }
    }

    public class KeyCloakUserPasswordResetRequest
    {
        public string Type => "password";
        public string Value { get; set; }
        public bool Temporary => false;

        public KeyCloakUserPasswordResetRequest(string value)
        {
            Value = value;
        }
    }
}
