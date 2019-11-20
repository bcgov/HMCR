using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserStatusDto
    {
        public const string ACTIVE = "ACTIVE";
        public const string INACTIVE = "INACTIVE";

        [JsonPropertyName("id")]
        public string UserStatusId { get; set; }
        [JsonPropertyName("name")]
        public string UserStatus { get; set; }
    }
}
