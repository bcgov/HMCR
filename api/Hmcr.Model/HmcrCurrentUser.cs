using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string BusinessNumber { get; set; }

        public UserCurrentDto UserInfo { get; set; }
    }
}
