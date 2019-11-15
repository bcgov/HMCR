using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.User
{
    public class UserStatusDto
    {
        public const string ACTIVE = "ACTIVE";
        public const string INACTIVE = "INACTIVE";

        public UserStatusDto()
        {
            UserStatus = new string[] { ACTIVE, INACTIVE };
        }

        public virtual IEnumerable<string> UserStatus { get; set; }
    }
}
