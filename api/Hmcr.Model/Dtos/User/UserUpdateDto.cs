using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserUpdateDto : IUserDto
    {
        public UserUpdateDto()
        {
            ServiceAreaNumbers = new List<decimal>();
            UserRoleIds = new List<decimal>();
            UserDirectory = "";
        }

        [JsonPropertyName("id")]
        public decimal SystemUserId { get; set; }
        public string UserType { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? EndDate { get; set; }
        public string UserDirectory { get; set; }

        public IList<decimal> ServiceAreaNumbers { get; set; }
        public IList<decimal> UserRoleIds { get; set; }
    }
}
