using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserDto
    {
        public UserDto()
        {
            ServiceAreaNumbers = new List<decimal>();
            UserRoleIds = new List<decimal>();
        }

        [JsonPropertyName("id")]
        public decimal SystemUserId { get; set; }
        public decimal PartyId { get; set; }
        public string Username { get; set; }
        public string UserDirectory { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BusinessLegalName { get; set; }
        public DateTime? EndDate { get; set; }
        public bool HasLogInHistory { get; set; }
        public virtual IList<decimal> ServiceAreaNumbers { get; set; }
        public virtual IList<decimal> UserRoleIds { get; set; }
    }
}
