using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.Permission;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.ServiceAreaUser;
using Hmcr.Model.Dtos.UserRole;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.User
{
    public class UserCurrentDto
    {
        public UserCurrentDto()
        {
            ServiceAreas = new List<ServiceAreaDto>();
            Roles = new List<UserCurrentRoleDto>();
            Permissions = new List<string>();
        }

        [JsonPropertyName("id")]
        public decimal SystemUserId { get; set; }   
        public decimal PartyId { get; set; }
        public string Username { get; set; }
        public string UserDirectory { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string BusinessLegalName { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive => EndDate == null || EndDate > DateTime.Today;
        public string AccountStatus => IsActive ? UserStatusDto.ACTIVE : UserStatusDto.INACTIVE;
        public bool IsSystemAdmin { get; set; }

        public virtual IList<ServiceAreaDto> ServiceAreas { get; set; }
        public virtual IList<UserCurrentRoleDto> Roles { get; set; }
        public virtual IList<string> Permissions { get; set; }       
    }
}
