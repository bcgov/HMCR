using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HrmSubmissionStatu
    {
        public HrmSubmissionStatu()
        {
            HmrSubmissionObjects = new HashSet<HmrSubmissionObject>();
        }

        public decimal SubmissionStatusId { get; set; }
        public string SubmissionStatusCode { get; set; }
        public string Description { get; set; }
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

        public virtual ICollection<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
    }
}
