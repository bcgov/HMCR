using System;

namespace Hmcr.Model.Dtos.RockfallReport
{
    public class RockfallReportTyped
    {
        public decimal RockfallReportId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal RowId { get; set; }
        public decimal? ValidationStatusId { get; set; }
        public string McrrIncidentNumber { get; set; }
        public string RecordType { get; set; }
        public decimal ServiceArea { get; set; }
        public DateTime? EstimatedRockfallDate { get; set; }
        public TimeSpan? EstimatedRockfallTime { get; set; }
        public decimal? StartLatitude { get; set; }
        public decimal? StartLongitude { get; set; }
        public decimal? EndLatitude { get; set; }
        public decimal? EndLongitude { get; set; }
        public string HighwayUnique { get; set; }
        public string HighwayUniqueName { get; set; }
        public string Landmark { get; set; }
        public string LandmarkName { get; set; }
        public decimal? StartOffset { get; set; }
        public decimal? EndOffset { get; set; }
        public string DirectionFromLandmark { get; set; }
        public string LocationDescription { get; set; }
        public string DitchVolume { get; set; }
        public string TravelledLanesVolume { get; set; }
        public decimal? OtherTravelledLanesVolume { get; set; }
        public decimal? OtherDitchVolume { get; set; }
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
