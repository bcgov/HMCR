using System;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserDto
    {
        [JsonPropertyName("id")]
        public decimal SystemUserId { get; set; }
        public decimal PartyId { get; set; }
        public string UserGuid { get; set; }
        public string Username { get; set; }
        public string UserDirectory { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Guid? BusinessGuid { get; set; }
        public string BusinessLegalName { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
