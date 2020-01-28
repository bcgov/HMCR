using Hmcr.Model.Dtos.User;
using System;

namespace Hmcr.Model
{
    public class HmcrCurrentUser
    {
        public Guid UserGuid { get; set; }
        public string UserType { get; set; }
        public string UniversalId { get; set; }
        public Guid? BusinessGuid { get; set; }
        public string AuthDirName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string BusinessLegalName { get; set; }
        public decimal BusinessNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserCurrentDto UserInfo { get; set; }
    }
}
