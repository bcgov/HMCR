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

        public UserStatusDto()
        {
            UserStatuses = new List<UserStatusDto>()
            {
                new UserStatusDto
                {
                    UserStatusId = 1,
                    UserStatus = ACTIVE
                },
                new UserStatusDto
                {
                    UserStatusId = 2,
                    UserStatus = INACTIVE
                }
            };
        }

        [JsonPropertyName("id")]
        public decimal UserStatusId { get; set; }
        [JsonPropertyName("name")]
        public string UserStatus { get; set; }

        public virtual IEnumerable<UserStatusDto> UserStatuses { get; set; }
    }
}
