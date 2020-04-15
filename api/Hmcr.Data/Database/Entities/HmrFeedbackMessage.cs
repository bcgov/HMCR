using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrFeedbackMessage
    {
        public decimal FeedbackMessageId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public string CommunicationSubject { get; set; }
        public string CommunicationText { get; set; }
        public DateTime? CommunicationDate { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsError { get; set; }
        public string SendErrorText { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual HmrSubmissionObject SubmissionObject { get; set; }
    }
}
