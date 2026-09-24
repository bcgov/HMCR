using System;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrNotificationPreference
    {
        public decimal NotificationPreferenceId { get; set; }
        public decimal ServiceAreaUserId { get; set; }
        public decimal SubmissionStreamId { get; set; }
        public bool SuccessEmailEnabled { get; set; }
        public bool ErrorEmailEnabled { get; set; }
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

        public virtual HmrServiceAreaUser ServiceAreaUser { get; set; }
        public virtual HmrSubmissionStream SubmissionStream { get; set; }
    }
}
