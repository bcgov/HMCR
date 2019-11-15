using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.User
{
    public class UserTypeDto
    {
        public const string INTERNAL = "INTERNAL";
        public const string BUSINESS = "BUSINESS";

        public UserTypeDto()
        {
            UserTypes = new string[] { INTERNAL, BUSINESS };
        }

        public virtual IEnumerable<string> UserTypes { get; set; }
    }
}
