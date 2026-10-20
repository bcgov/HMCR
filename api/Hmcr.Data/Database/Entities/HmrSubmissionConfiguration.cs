using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionConfiguration
    {
        public HmrSubmissionConfiguration()
        {
            Windows = new HashSet<HmrSubmissionConfigWindow>();
            Rules = new HashSet<HmrSubmissionConfigRule>();
            Audiences = new HashSet<HmrSubmissionConfigAudience>();
            ServiceAreas = new HashSet<HmrSubmissionConfigServiceArea>();
        }

        public decimal SubmissionConfigurationId { get; set; }
        public string ConfigurationKey { get; set; }
        public string ConfigurationName { get; set; }
        public decimal SubmissionStreamId { get; set; }
        public bool IsActive { get; set; }
        public DateTime EffectiveFromDate { get; set; }
        public DateTime? EffectiveToDate { get; set; }
        public string TimeZoneId { get; set; }
        public string ScopeType { get; set; }
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

        public virtual HmrSubmissionStream SubmissionStream { get; set; }
        public virtual ICollection<HmrSubmissionConfigWindow> Windows { get; set; }
        public virtual ICollection<HmrSubmissionConfigRule> Rules { get; set; }
        public virtual ICollection<HmrSubmissionConfigAudience> Audiences { get; set; }
        public virtual ICollection<HmrSubmissionConfigServiceArea> ServiceAreas { get; set; }
    }
}
