using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrWildlifeReportVw
    {
        public decimal WildlifeRecordId { get; set; }
        public string ReportType { get; set; }
        public string RecordType { get; set; }
        public decimal? ServiceArea { get; set; }
        public DateTime? AccidentDate { get; set; }
        public string TimeOfKill { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? SpatialVariance { get; set; }
        public decimal? WarningSpThreshold { get; set; }
        public string IsOverSpThreshold { get; set; }
        public string HighwayUnique { get; set; }
        public string HighwayUniqueName { get; set; }
        public decimal? HighwayUniqueLength { get; set; }
        public string Landmark { get; set; }
        public decimal? Offset { get; set; }
        public string NearestTown { get; set; }
        public string WildlifeSign { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Species { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public string Comment { get; set; }
        public Geometry Geometry { get; set; }
        public decimal? SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public decimal? RowNum { get; set; }
        public string ValidationStatus { get; set; }
        public DateTime? AppCreateTimestampUtc { get; set; }
        public DateTime? AppLastUpdateTimestampUtc { get; set; }
        public DateTime? SubmissionDate { get; set; }
    }
}
