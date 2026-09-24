using System;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionConfigWindow
    {
        public decimal SubmissionConfigWindowId { get; set; }
        public decimal SubmissionConfigurationId { get; set; }
        public string WindowType { get; set; }
        public byte StartMonth { get; set; }
        public byte StartDay { get; set; }
        public byte EndMonth { get; set; }
        public byte EndDay { get; set; }
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
    }
}
