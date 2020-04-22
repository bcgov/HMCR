using System.Collections.Generic;

namespace Hmcr.Model.Dtos.Keycloak
{
    public class KeycloakClientCreateDto
    {
        public bool Enabled { get; set; } = true;
        public string ClientId { get; set; }
        public string Protocol { get; set; } = KeycloakMapperConfig.DefaultProtocol;
        public bool FullScopeAllowed { get; set; } = false;
        public bool StandardFlowEnabled { get; set; } = false;
        public bool ImplicitFlowEnabled { get; set; } = false;
        public bool DirectAccessGrantsEnabled { get; set; } = false;
        public bool PublicClient { get; set; } = false;
        public bool ServiceAccountsEnabled { get; set; } = true;
        public List<KeycloakProtocolMapperDto> ProtocolMappers { get; set; }

        public KeycloakClientCreateDto()
        {
            ProtocolMappers = new List<KeycloakProtocolMapperDto>();
        }

        public KeycloakClientCreateDto(string audience, string username, string email, string guidClaimName, string guidClaimValue)
        {
            ProtocolMappers = new List<KeycloakProtocolMapperDto>();

            AddAudienceMapper(audience);
            AddApiClientClaimMapper();
            AddHardcodedClaimMapper("email", email.ToLowerInvariant(), "String");
            AddHardcodedClaimMapper("username", username.ToLowerInvariant(), "String");
            AddHardcodedClaimMapper(guidClaimName, guidClaimValue, "String");
        }

        public void AddAudienceMapper(string audience)
        {
            var protocolMapper = new KeycloakProtocolMapperDto()
            {
                Name = audience,
                ProtocolMapper = KeycloakMapperConfig.OidcAudienceMapper
            };

            protocolMapper.Config[KeycloakMapperConfig.IncludedClientAudience] = audience;
            protocolMapper.Config[KeycloakMapperConfig.IncludedCustomAudience] = audience;

            ProtocolMappers.Add(protocolMapper);
        }

        public void AddHardcodedClaimMapper(string name, string value, string jsonType)
        {
            var protocolMapper = new KeycloakProtocolMapperDto()
            {
                Name = name,
                ProtocolMapper = KeycloakMapperConfig.OidcHardcodedClaimMapper
            };

            protocolMapper.Config[KeycloakMapperConfig.ClaimName] = name;
            protocolMapper.Config[KeycloakMapperConfig.ClaimValue] = value;
            protocolMapper.Config[KeycloakMapperConfig.JsonTypeLabel] = jsonType;

            ProtocolMappers.Add(protocolMapper);
        }

        public void AddApiClientClaimMapper()
        {
            var protocolMapper = new KeycloakProtocolMapperDto()
            {
                Name = KeycloakMapperConfig.ApiClient,
                ProtocolMapper = KeycloakMapperConfig.OidcHardcodedClaimMapper
            };

            protocolMapper.Config[KeycloakMapperConfig.ClaimName] = KeycloakMapperConfig.ApiClient;
            protocolMapper.Config[KeycloakMapperConfig.ClaimValue] = "true";
            protocolMapper.Config[KeycloakMapperConfig.JsonTypeLabel] = "boolean";

            ProtocolMappers.Add(protocolMapper);
        }
    }


}
