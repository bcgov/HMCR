using System;

namespace Hmcr.Model.Dtos.RockfallReport
{
    public class RockfallReportCsvDto : IReportCsvDto
    {
        public decimal RowId { get; set; }
        public string McrrIncidentNumber { get; set; }
        public string RecordType { get; set; }
        public string ServiceArea { get; set; }
        public DateTime? EstimatedRockfallDate { get; set; }
        public string EstimatedRockfallTime { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string EndLatitude { get; set; }
        public string EndLongitude { get; set; }
        public string HighwayUnique { get; set; }
        public string HighwayUniqueName { get; set; }
        public string Landmark { get; set; }
        public string LandmarkName { get; set; }
        public string StartOffset { get; set; }
        public string EndOffset { get; set; }
        public string DirectionFromLandmark { get; set; }
        public string LocationDescription { get; set; }
        public string DitchVolume { get; set; }
        public string TravelledLanesVolume { get; set; }
        public string OtherTravelledLanesVolume { get; set; }
        public string OtherDitchVolume { get; set; }
        public string HeavyPrecip { get; set; }
        public string FreezeThaw { get; set; }
        public string DitchSnowIce { get; set; }
        public string VehicleDamage { get; set; }
        public string Comments { get; set; }
        public string Name { get; set; }
        public string McName { get; set; }
        public string McPhoneNumber { get; set; }
        public DateTime? ReportDate { get; set; }
        public decimal? RowNum { get; set; }
        public SpatialData SpatialData { get; set; }
    }
}
