using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.UserRole;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.User
{
    public class UserDto
    {
        public UserDto()
        {
            ServiceAreaUsers = new HashSet<ServiceAreaUserDto>();
            UserRoles = new HashSet<UserRoleDto>();
        }

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

        public virtual PartyDto Party { get; set; }
        public virtual ICollection<ServiceAreaUserDto> ServiceAreaUsers { get; set; }
        public virtual ICollection<UserRoleDto> UserRoles { get; set; }
    }
}
