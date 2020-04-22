using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSystemUser
    {
        public HmrSystemUser()
        {
            HmrServiceAreaUsers = new HashSet<HmrServiceAreaUser>();
            HmrUserRoles = new HashSet<HmrUserRole>();
        }

        public decimal SystemUserId { get; set; }
        public decimal? PartyId { get; set; }
        public string ApiClientId { get; set; }
        public Guid? UserGuid { get; set; }
        public string Username { get; set; }
        public string UserDirectory { get; set; }
        public string UserType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Guid? BusinessGuid { get; set; }
        public string BusinessLegalName { get; set; }
        public string ApiClientId { get; set; }
        public DateTime? EndDate { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string AppCreateUserid { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public Guid AppCreateUserGuid { get; set; }
        public string AppCreateUserDirectory { get; set; }
        public string AppLastUpdateUserid { get; set; }
        public DateTime AppLastUpdateTimestamp { get; set; }
        public Guid AppLastUpdateUserGuid { get; set; }
        public string AppLastUpdateUserDirectory { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual HmrParty Party { get; set; }
        public virtual ICollection<HmrServiceAreaUser> HmrServiceAreaUsers { get; set; }
        public virtual ICollection<HmrUserRole> HmrUserRoles { get; set; }
    }
}
