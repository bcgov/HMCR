using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrParty
    {
        public HmrParty()
        {
            HmrContractTerms = new HashSet<HmrContractTerm>();
            HmrSubmissionObjects = new HashSet<HmrSubmissionObject>();
            HmrSystemUsers = new HashSet<HmrSystemUser>();
        }

        public decimal PartyId { get; set; }
        public string BusinessGuid { get; set; }
        public string BusinessLegalName { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public decimal? BusinessNumber { get; set; }
        public string PartyType { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal ConcurrencyControlNumber { get; set; }
        public string AppCreateUserid { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public string AppCreateUserGuid { get; set; }
        public string AppCreateUserDirectory { get; set; }
        public string AppLastUpdateUserid { get; set; }
        public DateTime AppLastUpdateTimestamp { get; set; }
        public string AppLastUpdateUserGuid { get; set; }
        public string AppLastUpdateUserDirectory { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual ICollection<HmrContractTerm> HmrContractTerms { get; set; }
        public virtual ICollection<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
        public virtual ICollection<HmrSystemUser> HmrSystemUsers { get; set; }
    }
}
