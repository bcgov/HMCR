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

        public UserTypeDto()
        {
            UserTypes = new List<UserTypeDto>()
            {
                new UserTypeDto
                {
                    UserTypeId = INTERNAL,
                    UserType = INTERNAL
                },
                new UserTypeDto
                {
                    UserTypeId = BUSINESS,
                    UserType = BUSINESS
                }
            };
        }

        [JsonPropertyName("id")]
        public string UserTypeId { get; set; }
        [JsonPropertyName("name")]
        public string UserType { get; set; }

        public virtual IEnumerable<UserTypeDto> UserTypes { get; set; }
    }
}
