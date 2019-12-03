using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionStream
    {
        public HmrSubmissionStream()
        {
            HmrStreamElements = new HashSet<HmrStreamElement>();
            HmrSubmissionObjects = new HashSet<HmrSubmissionObject>();
        }

        public decimal SubmissionStreamId { get; set; }
        public string StreamName { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? FileSizeLimit { get; set; }
        public string StagingTableName { get; set; }
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

        public virtual ICollection<HmrStreamElement> HmrStreamElements { get; set; }
        public virtual ICollection<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
    }
}
