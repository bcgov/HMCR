using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionConfigRule
    {
        public HmrSubmissionConfigRule()
        {
            Activities = new HashSet<HmrSubmissionConfigRuleActivity>();
        }

        public decimal SubmissionConfigRuleId { get; set; }
        public decimal SubmissionConfigurationId { get; set; }
        public string RuleType { get; set; }
        public string DisplayLabel { get; set; }
        public string ComparisonOperator { get; set; }
        public decimal ThresholdValue { get; set; }
        public string UnitOfMeasure { get; set; }
        public short DisplayOrder { get; set; }
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

        public virtual HmrSubmissionConfiguration SubmissionConfiguration { get; set; }
        public virtual ICollection<HmrSubmissionConfigRuleActivity> Activities { get; set; }
    }
}
