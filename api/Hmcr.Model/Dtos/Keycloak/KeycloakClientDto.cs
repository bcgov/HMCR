using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.Keycloak
{
    public class KeycloakClientDto
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
