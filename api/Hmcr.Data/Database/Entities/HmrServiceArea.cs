using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrServiceArea
    {
        public HmrServiceArea()
        {
            HmrContractTerms = new HashSet<HmrContractTerm>();
            HmrServiceAreaUsers = new HashSet<HmrServiceAreaUser>();
            HmrSubmissionObjects = new HashSet<HmrSubmissionObject>();
        }

        public decimal ServiceAreaId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public string ServiceAreaName { get; set; }
        public decimal DistrictNumber { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual HmrDistrict DistrictNumberNavigation { get; set; }
        public virtual ICollection<HmrContractTerm> HmrContractTerms { get; set; }
        public virtual ICollection<HmrServiceAreaUser> HmrServiceAreaUsers { get; set; }
        public virtual ICollection<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
    }
}
