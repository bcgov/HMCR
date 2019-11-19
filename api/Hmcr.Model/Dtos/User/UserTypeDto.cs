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
                    UserTypeId = 1,
                    UserType = INTERNAL
                },
                new UserTypeDto
                {
                    UserTypeId = 2,
                    UserType = BUSINESS
                }
            };
        }

        [JsonPropertyName("id")]
        public decimal UserTypeId { get; set; }
        [JsonPropertyName("name")]
        public string UserType { get; set; }

        public virtual IEnumerable<UserTypeDto> UserTypes { get; set; }
    }
}
