using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.UserRole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.User
{
    public abstract class UserCreateDto
    {
        public UserCreateDto()
        {
            ServiceAreaUsers = new HashSet<ServiceAreaUserDto>();
            UserRoles = new HashSet<UserRoleDto>();
        }

        public string Username { get; set; }
        public string UserDirectory { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? EndDate { get; set; }

        public virtual ICollection<ServiceAreaUserDto> ServiceAreaUsers { get; set; }
        public virtual ICollection<UserRoleDto> UserRoles { get; set; }
    }
}
