using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrWorkReport
    {
        public decimal WorkReportId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal? RowNum { get; set; }
        public decimal? ValidationStatusId { get; set; }
        public string RecordType { get; set; }
        public decimal ServiceArea { get; set; }
        public string RecordNumber { get; set; }
        public string TaskNumber { get; set; }
        public string ActivityNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Accomplishment { get; set; }
        public string UnitOfMeasure { get; set; }
        public DateTime? PostedDate { get; set; }
        public string HighwayUnique { get; set; }
        public string Landmark { get; set; }
        public decimal? StartOffset { get; set; }
        public decimal? EndOffset { get; set; }
        public decimal? StartLatitude { get; set; }
        public decimal? StartLongitude { get; set; }
        public decimal? EndLatitude { get; set; }
        public decimal? EndLongitude { get; set; }
        public string StructureNumber { get; set; }
        public string SiteNumber { get; set; }
        public decimal? ValueOfWork { get; set; }
        public string Comments { get; set; }
        public Geometry Geometry { get; set; }
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

        public virtual HmrServiceArea ServiceAreaNavigation { get; set; }
        public virtual HmrSubmissionObject SubmissionObject { get; set; }
        public virtual HmrSubmissionStatu ValidationStatus { get; set; }
    }
}
