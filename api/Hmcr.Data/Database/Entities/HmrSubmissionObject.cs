using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionObject
    {
        public HmrSubmissionObject()
        {
            HmrFeedbackMessages = new HashSet<HmrFeedbackMessage>();
            HmrRockfallReports = new HashSet<HmrRockfallReport>();
            HmrSubmissionRows = new HashSet<HmrSubmissionRow>();
            HmrWildlifeReports = new HashSet<HmrWildlifeReport>();
            HmrWorkReports = new HashSet<HmrWorkReport>();
        }

        public decimal SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public byte[] DigitalRepresentation { get; set; }
        public decimal MimeTypeId { get; set; }
        public decimal? ContractTermId { get; set; }
        public decimal SubmissionStatusId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal? PartyId { get; set; }
        public string FileHash { get; set; }
        public string ErrorDetail { get; set; }
        public decimal SubmissionStreamId { get; set; }
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

        public virtual HmrContractTerm ContractTerm { get; set; }
        public virtual HmrMimeType MimeType { get; set; }
        public virtual HmrParty Party { get; set; }
        public virtual HmrServiceArea ServiceAreaNumberNavigation { get; set; }
        public virtual HmrSubmissionStatu SubmissionStatus { get; set; }
        public virtual HmrSubmissionStream SubmissionStream { get; set; }
        public virtual ICollection<HmrFeedbackMessage> HmrFeedbackMessages { get; set; }
        public virtual ICollection<HmrRockfallReport> HmrRockfallReports { get; set; }
        public virtual ICollection<HmrSubmissionRow> HmrSubmissionRows { get; set; }
        public virtual ICollection<HmrWildlifeReport> HmrWildlifeReports { get; set; }
        public virtual ICollection<HmrWorkReport> HmrWorkReports { get; set; }
    }
}
