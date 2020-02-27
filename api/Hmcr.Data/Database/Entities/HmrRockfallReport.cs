using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrRockfallReport
    {
        public decimal RockfallReportId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal RowId { get; set; }
        public decimal? RowNum { get; set; }
        public decimal? ValidationStatusId { get; set; }
        public string McrrIncidentNumber { get; set; }
        public string RecordType { get; set; }
        public decimal? ServiceArea { get; set; }
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
        public string ReporterName { get; set; }
        public string McPhoneNumber { get; set; }
        public DateTime? ReportDate { get; set; }
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

        public virtual HmrSubmissionRow Row { get; set; }
        public virtual HmrServiceArea ServiceAreaNavigation { get; set; }
        public virtual HmrSubmissionObject SubmissionObject { get; set; }
        public virtual HmrSubmissionStatu ValidationStatus { get; set; }
    }
}
