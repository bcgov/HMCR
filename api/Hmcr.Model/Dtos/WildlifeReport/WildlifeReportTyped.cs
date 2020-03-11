using System;

namespace Hmcr.Model.Dtos.WildlifeReport
{
    public class WildlifeReportTyped
    {
        public decimal WildlifeRecordId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal RowId { get; set; }
        public decimal? ValidationStatusId { get; set; }
        public string RecordType { get; set; }
        public decimal ServiceArea { get; set; }
        public DateTime? AccidentDate { get; set; }
        public string TimeOfKill { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
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
        public decimal? RowNum { get; set; }
        public SpatialData SpatialData { get; set; }
    }
}
