using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrWildlifeReportVw
    {
        public string ReportType { get; set; }
        public string RecordType { get; set; }
        public decimal ServiceArea { get; set; }
        public DateTime? AccidentDate { get; set; }
        public string TimeOfKill { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string IsOverSpTolerance { get; set; }
        public string HighwayUnique { get; set; }
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
        public decimal SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public decimal WildlifeRecordId { get; set; }
        public decimal? RowNum { get; set; }
        public string ValidationStatus { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public DateTime AppLastUpdateTimestamp { get; set; }
    }
}
