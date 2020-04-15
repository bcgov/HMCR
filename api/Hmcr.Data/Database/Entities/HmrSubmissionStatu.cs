using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionStatu
    {
        public HmrSubmissionStatu()
        {
            HmrRockfallReports = new HashSet<HmrRockfallReport>();
            HmrSubmissionObjects = new HashSet<HmrSubmissionObject>();
            HmrSubmissionRows = new HashSet<HmrSubmissionRow>();
            HmrWildlifeReports = new HashSet<HmrWildlifeReport>();
            HmrWorkReports = new HashSet<HmrWorkReport>();
        }

        public decimal StatusId { get; set; }
        public string StatusCode { get; set; }
        public string Description { get; set; }
        public string LongDescription { get; set; }
        public string StatusType { get; set; }
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

        public virtual ICollection<HmrRockfallReport> HmrRockfallReports { get; set; }
        public virtual ICollection<HmrSubmissionObject> HmrSubmissionObjects { get; set; }
        public virtual ICollection<HmrSubmissionRow> HmrSubmissionRows { get; set; }
        public virtual ICollection<HmrWildlifeReport> HmrWildlifeReports { get; set; }
        public virtual ICollection<HmrWorkReport> HmrWorkReports { get; set; }
    }
}
