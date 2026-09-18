using System;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionConfigRuleActivity
    {
        public decimal SubmissionConfigRuleActivityId { get; set; }
        public decimal SubmissionConfigRuleId { get; set; }
        public decimal ActivityCodeId { get; set; }
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

        public virtual HmrSubmissionConfigRule SubmissionConfigRule { get; set; }
        public virtual HmrActivityCode ActivityCode { get; set; }
    }
}
