namespace Hmcr.Model.Dtos.WildlifeReport
{
    public class WildlifeReportCsvDto : IReportCsvDto
    {
        public decimal RowId { get; set; }
        public string RecordType { get; set; }
        public string ServiceArea { get; set; }
        public string AccidentDate { get; set; }
        public string TimeOfKill { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string HighwayUnique { get; set; }
        public string Landmark { get; set; }
        public string Offset { get; set; }
        public string NearestTown { get; set; }
        public string WildlifeSign { get; set; }
        public string Quantity { get; set; }
        public string Species { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public string Comment { get; set; }
        public decimal? RowNum { get; set; }

    }
}
