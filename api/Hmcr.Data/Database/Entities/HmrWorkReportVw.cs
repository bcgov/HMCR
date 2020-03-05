using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrWorkReportVw
    {
        public string ReportType { get; set; }
        public string RecordType { get; set; }
        public decimal? ServiceAreaNumber { get; set; }
        public string RecordNumber { get; set; }
        public string TaskNumber { get; set; }
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Accomplishment { get; set; }
        public string UnitOfMeasure { get; set; }
        public DateTime? PostedDate { get; set; }
        public string HighwayUnique { get; set; }
        public string HighwayUniqueName { get; set; }
        public decimal? HighwayUniqueLength { get; set; }
        public string Landmark { get; set; }
        public decimal? StartOffset { get; set; }
        public decimal? EndOffset { get; set; }
        public decimal? StartLatitude { get; set; }
        public decimal? StartLongitude { get; set; }
        public decimal? StartVariance { get; set; }
        public decimal? EndLatitude { get; set; }
        public decimal? EndLongitude { get; set; }
        public decimal? EndVariance { get; set; }
        public decimal? WorkLength { get; set; }
        public string IsOverSpTolerance { get; set; }
        public string StructureNumber { get; set; }
        public string SiteNumber { get; set; }
        public decimal? ValueOfWork { get; set; }
        public string Comments { get; set; }
        public Geometry Geometry { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public string SubmissionFileName { get; set; }
        public decimal WorkReportId { get; set; }
        public decimal? RowNum { get; set; }
        public string ValidationStatus { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public DateTime AppLastUpdateTimestamp { get; set; }
    }
}
