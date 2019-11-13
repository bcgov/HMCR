using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionObject
    {
        public decimal SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public byte[] DigitalRepresentation { get; set; }
        public decimal MimeTypeId { get; set; }
        public decimal SubmissionStatusId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal PartyId { get; set; }
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

        public virtual HmrMimeType MimeType { get; set; }
        public virtual HmrParty Party { get; set; }
        public virtual HmrServiceArea ServiceAreaNumberNavigation { get; set; }
        public virtual HrmSubmissionStatu SubmissionStatus { get; set; }
    }
}
