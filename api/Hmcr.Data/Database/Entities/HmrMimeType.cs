using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrMimeType
    {
        public HmrMimeType()
        {
            HmrSubmissionObjects = new HashSet<HmrSubmissionObject>();
        }

        public decimal MimeTypeId { get; set; }
        public string MimeTypeCode { get; set; }
        public string Description { get; set; }
        public decimal ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual ICollection<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
    }
}
