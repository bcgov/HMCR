using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserTypeDto
    {
        public const string INTERNAL = "INTERNAL";
        public const string BUSINESS = "BUSINESS";
        public const string IDIR = "IDIR"; //user directory which is used as description of user type internal
        public const string BCeId = "BCeID"; //user directory which is used as description of user type business

        [JsonPropertyName("id")]
        public string UserTypeId { get; set; }
        [JsonPropertyName("name")]
        public string UserType { get; set; }
    }
}
