using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.UserRole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.User
{
    public class UserCreateDto
    {
        public UserCreateDto()
        {
            ServiceAreaNumbers = new List<decimal>();
            UserRoleIds = new List<decimal>();
            UserDirectory = "";
        }

        public string UserType { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? EndDate { get; set; }
        public string UserDirectory { get; set; }

        public virtual IList<decimal> ServiceAreaNumbers { get; set; }
        public virtual IList<decimal> UserRoleIds { get; set; }
    }
}
