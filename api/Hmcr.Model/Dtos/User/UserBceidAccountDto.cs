using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserBceidAccountDto
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessLegalName { get; set; }
        public decimal BusinessNumber { get; set; }
        public string DoingBusinessAs { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string DisplayName { get; set; }
    }
}
