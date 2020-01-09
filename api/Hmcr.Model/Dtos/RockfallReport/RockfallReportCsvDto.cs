using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.RockfallReport
{
    public class RockfallReportCsvDto
    {
        public decimal RowId { get; set; }
        public int RowNumber { get; set; }
        public string MajorIncidentNumber { get; set; }
        public string EstimatedRockfallDate { get; set; }
        public string EstimatedRockfallTime { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string EndLatitude { get; set; }
        public string EndLongitude { get; set; }
        public string HighwayUniqueNumber { get; set; }
        public string HighwayUniqueName { get; set; }
        public string Landmark { get; set; }
        public string LandMarkName { get; set; }
        public string StartOffset { get; set; }
        public string EndOffset { get; set; }
        public string DirectionFromLandmark { get; set; }
        public string LocationDescription { get; set; }
        public string DitchVolume { get; set; }
        public string TravelledLanesVolume { get; set; }
        public string OtherVolume { get; set; }
        public string HeavyPrecip { get; set; }
        public string FreezeThaw { get; set; }
        public string DitchSnowIce { get; set; }
        public string VehicleDamage { get; set; }
        public string Comments { get; set; }
        public string ReporterName { get; set; }
        public string McPhoneNumber { get; set; }
        public string ReportDate { get; set; }
    }
}
