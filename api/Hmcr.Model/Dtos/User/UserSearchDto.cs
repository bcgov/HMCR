using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserSearchDto
    {
        [JsonPropertyName("id")]
        public decimal SystemUserId { get; set; }
        public string UserType { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public string BusinessLegalName { get; set; }
        public string ServiceAreas { get; set; }
        public bool HasLogInHistory { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive => EndDate == null || EndDate > DateTime.Today;
    }
}
