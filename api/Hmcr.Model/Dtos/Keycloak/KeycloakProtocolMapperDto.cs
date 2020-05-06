using System.Collections.Generic;

namespace Hmcr.Model.Dtos.Keycloak
{
    public class KeycloakProtocolMapperDto
    {
        public string Name { get; set; }
        public string Protocol { get; set; } = KeycloakMapperConfig.DefaultProtocol;
        public string ProtocolMapper { get; set; }
        public Dictionary<string, string> Config { get; set; }

        public KeycloakProtocolMapperDto()
        {
            Config = new Dictionary<string, string>();
            Config[KeycloakMapperConfig.AccessTokenClaim] = "true";            
        }
    }
}
