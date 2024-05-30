namespace Hmcr.Model.Dtos.SaltReport
{
    public class VulnareaDto
    {
        public decimal VulnerableAreaId { get; set; }
        public decimal SaltReportId { get; set; }
        public string HighwayNumber { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Feature { get; set; }
        public string Type { get; set; }
        public string ProtectionMeasures { get; set; }
        public bool? EnvironmentalMonitoring { get; set; }
        public string Comments { get; set; }
    }
}
