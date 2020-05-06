using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSubmissionRow
    {
        public HmrSubmissionRow()
        {
            HmrRockfallReports = new HashSet<HmrRockfallReport>();
            HmrWildlifeReports = new HashSet<HmrWildlifeReport>();
            HmrWorkReports = new HashSet<HmrWorkReport>();
        }

        public decimal RowId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal? RowStatusId { get; set; }
        public decimal? RowNum { get; set; }
        public string RecordNumber { get; set; }
        public string RowValue { get; set; }
        public string RowHash { get; set; }
        public decimal? StartVariance { get; set; }
        public decimal? EndVariance { get; set; }
        public bool? IsResubmitted { get; set; }
        public string ErrorDetail { get; set; }
        public string WarningDetail { get; set; }
        public decimal? WarningSpThreshold { get; set; }
        public decimal? ErrorSpThreshold { get; set; }
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

        public virtual HmrSubmissionStatu RowStatus { get; set; }
        public virtual HmrSubmissionObject SubmissionObject { get; set; }
        public virtual ICollection<HmrRockfallReport> HmrRockfallReports { get; set; }
        public virtual ICollection<HmrWildlifeReport> HmrWildlifeReports { get; set; }
        public virtual ICollection<HmrWorkReport> HmrWorkReports { get; set; }
    }
}
