﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model
{
    public static class Constants
    {
        public static DateTime MaxDate = new DateTime(9999, 12, 31);
        public static DateTime MinDate = new DateTime(1900, 1, 1);
        public static decimal MaxFileSize = 2097152;
        public const string VancouverTimeZone = "America/Vancouver";
        public const string PacificTimeZone = "Pacific Standard Time";
        public const string SystemAdmin = "SYSTEM_ADMIN";
        public const string HmcrOrigins = "HmcrOrigins";
    }

    public static class Reports
    {
        public const string Work = "Work";
        public const string Rockfall = "Rockfall";
        public const string Wildlife = "Wildlife";
    }

    public static class ExportQuery
    {
        public const string CqlFilter = "cql_filter";
        public const string ServiceAreas = "serviceAreas";
        public const string OutputFormat = "outputFormat";
        public const string Format = "format";
        public const string TypeName = "typeName";
        public const string Layers = "layers";
        public const string FromDate = "fromDate";
        public const string ToDate = "toDate";
        public const string Count = "count";
    }

    public static class ExportQueryEndpointConfigName
    {
        public const string WFS = "WFSExportPath";
        public const string WMS = "KMLExportPath";
    }

    public static class Permissions
    {
        public const string CodeWrite = "CODE_W";
        public const string CodeRead = "CODE_R";
        public const string UserWrite = "USER_W";
        public const string UserRead = "USER_R";
        public const string RoleWrite = "ROLE_W";
        public const string RoleRead = "ROLE_R";
        public const string FileUploadWrite = "FILE_W";
        public const string FileUploadRead = "FILE_R";
        public const string Export = "EXPORT";
    }

    public static class Entities
    {
        public const string User = "user";
        public const string Role = "role";

        public const string WorkReportInit = "worki";
        public const string WorkReportD2 = "workd2";
        public const string WorkReportD3 = "workd3";
        public const string WorkReportD4 = "workd4";

        public const string WorkReportHighwayUnique = "wrokhu";
        public const string WorkReportSite = "worksite";
        public const string WorkReportStructure = "workstructure";
        public const string WorkReportValueOfWork = "worvow";

        public const string RockfallReportInit = "rockfalli";
        public const string RockfallReport = "rockfall";
        public const string RockfallReportGps = "rockfallgps";
        public const string RockfallReportLrs = "rockfalllrs";

        public const string RockfallReportOtherDitchVolume = "rockfallotherditchvolume";
        public const string RockfallReportOtherTravelledLanesVolume = "rockfallothertravelledlanesvolume";

        public const string WildlifeReportInit = "wildlifei";
        public const string WildlifeReport = "wildlife";
        public const string WildlifeReportGps = "wildlifegps";
        public const string WildlifeReportLrs = "wildlifelrs";

        public const string ActivityCode = "activitycode";
        public const string ActivityCodeLocationCodeC = "activitylocc";
    }

    public static class FieldTypes
    {
        public const string String = "S";
        public const string Decimal = "N";
        public const string Date = "D";
    }

    public static class TableNames
    {
        public const string WorkReport = "HMR_WORK_REPORT";
        public const string RockfallReport = "HMR_ROCKFALL_REPORT";
        public const string WildlifeReport = "HMR_WILDLIFE_REPORT";
        public const string SaltReport = "HMR_SALT_REPORT";
    }

    public static class StatusType
    {
        public const string File = "F";
        public const string Row = "R";
    }

    public static class RowStatus
    {
        public const string RowReceived = "RR";
        public const string RowDuplicate = "DR";
        public const string RowError = "RE";
        public const string RowSuccess = "RS";
    }

    public static class FileStatus
    {
        public const string FileReceived = "FR";
        public const string FileError = "FE";
        public const string FileDuplicate = "DS";
        public const string FileInProgress = "DP";
        public const string FileBasicError = "DE";
        public const string FileConflictionError = "S3";
        public const string FileLocationError = "S4";
        public const string FileUnexpectedError = "UE";
        public const string FileSuccess = "VS";
        public const string FileStage3InProgress = "3P";
        public const string FileStage4InProgress = "4P";
        public const string FileServiceAreaError = "3E";
        public const string FileSuccessWithWarnings = "SW";
    }

    public static class Fields
    {
        public const string Username = "Username";
        public const string UserType = "UserType";
        public const string UserDirectory = "UserDirectory";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string Email = "Email";
        public const string EndDate = "EndDate";
        public const string IsInternal = "IsInternal";

        public const string ServiceAreaNumber = "ServiceAreaNumber";

        public const string RoleId = "RoleId";
        public const string Name = "Name";
        public const string Description = "Description";

        public const string PermissionId = "PermissionId";

        public const string RecordType = "RecordType";
        public const string RecordNumber = "RecordNumber";
        public const string ServiceArea = "ServiceArea";
        public const string TaskNumber = "TaskNumber";
        public const string ActivityNumber = "ActivityNumber";
        public const string StartDate = "StartDate";
        public const string Accomplishment = "Accomplishment";
        public const string UnitOfMeasure = "UnitOfMeasure";
        public const string PostedDate = "PostedDate";
        public const string HighwayUnique = "HighwayUnique";
        public const string Landmark = "Landmark";
        public const string StartOffset = "StartOffset";
        public const string EndOffset = "EndOffset";
        public const string StartLatitude = "StartLatitude";
        public const string StartLongitude = "StartLongitude";
        public const string EndLatitude = "EndLatitude";
        public const string EndLongitude = "EndLongitude";
        public const string StructureNumber = "StructureNumber";
        public const string SiteNumber = "SiteNumber";
        public const string ValueOfWork = "ValueOfWork";
        public const string Comments = "Comments";

        public const string McrrIncidentNumber = "McrrIncidentNumber";
        public const string EstimatedRockfallDate = "EstimatedRockfallDate";
        public const string EstimatedRockfallTime = "EstimatedRockfallTime";
        public const string HighwayUniqueName = "HighwayUniqueName";
        public const string LandmarkName = "LandmarkName";
        public const string DirectionFromLandmark = "DirectionFromLandmark";
        public const string DitchVolume = "DitchVolume";
        public const string TravelledLanesVolume = "TravelledLanesVolume";
        public const string HeavyPrecip = "HeavyPrecip";
        public const string FreezeThaw = "FreezeThaw";
        public const string DitchSnowIce = "DitchSnowIce";
        public const string VehicleDamage = "VehicleDamage";
        public const string McPhoneNumber = "McPhoneNumber";
        public const string McName = "McName";
        public const string ReportDate = "ReportDate";
        public const string LocationDescription = "LocationDescription";
        public const string OtherTravelledLanesVolume = "OtherTravelledLanesVolume";
        public const string OtherDitchVolume = "OtherDitchVolume";

        public const string AccidentDate = "AccidentDate";
        public const string TimeOfKill = "TimeOfKill";
        public const string Latitude = "Latitude";
        public const string Longitude = "Longitude";
        public const string Offset = "Offset";
        public const string NearestTown = "NearestTown";
        public const string WildlifeSign = "WildlifeSign";
        public const string Quantity = "Quantity";
        public const string Species = "Species";
        public const string Sex = "Sex";
        public const string Age = "Age";

        public const string RowNum = "RowNum";

        public const string WorkReportId = "WorkReportId";
        public const string SubmissionObjectId = "SubmissionObjectId";

        public const string WildlifeRecordId = "WildlifeRecordId";
        public const string RockfallReportId = "RockfallReportId";

        public const string ActivityName = "ActivityName";
        public const string MaintenanceType = "MaintenanceType";
        public const string FeatureType = "FeatureType";
        public const string LocationCodeId = "LocationCodeId";
        public const string SpThresholdLevel = "SpThresholdLevel";

        public const string ApiClientId = "ApiClientId";
    }

    public interface IReportHeaders
    {
        string[] CommonMandatoryFields { get; }
    }
    public class WorkReportHeaders : IReportHeaders
    {

        public static string[] MandatoryFields = new string[]
        {
            Fields.RecordType, Fields.ServiceArea, Fields.RecordNumber, Fields.ActivityNumber,
            Fields.EndDate, Fields.Accomplishment, Fields.UnitOfMeasure, Fields.PostedDate
        };
        public string[] CommonMandatoryFields => MandatoryFields;
    }

    public class RockfallReportHeaders : IReportHeaders
    {
        public static string[] MandatoryFields = new string[]
        {
            Fields.RecordType, Fields.ServiceArea, Fields.McrrIncidentNumber, Fields.EstimatedRockfallDate, Fields.EstimatedRockfallTime,
            Fields.HighwayUnique, Fields.DitchVolume, Fields.TravelledLanesVolume, Fields.HeavyPrecip, Fields.FreezeThaw, Fields.DitchSnowIce,
            Fields.VehicleDamage, Fields.Name, Fields.McPhoneNumber, Fields.ReportDate
        };
        public string[] CommonMandatoryFields => MandatoryFields;
    }

    public class WildlifeReportHeaders : IReportHeaders
    {
        public static string[] MandatoryFields = new string[]
        {
            Fields.RecordType, Fields.ServiceArea, Fields.AccidentDate, Fields.TimeOfKill, Fields.HighwayUnique,
            Fields.WildlifeSign, Fields.Quantity, Fields.Species, Fields.Sex, Fields.Age
        };
        public string[] CommonMandatoryFields => MandatoryFields;
    }

    public class DateColNames
    {
        public const string EndDate = "END_DATE";
        public const string ReportDate = "REPORT_DATE";
        public const string AccidentDate = "ACCIDENT_DATE";
        public const string RockfallDate = "ESTIMATED_ROCKFALL_DATE";
    }

    public static class CodeSet
    {
        public const string UnitOfMeasure = "UOM";
        public const string WarSpecies = "WARS_SPECIES";
        public const string WarsTime = "WARS_TIME";
        public const string WarsSex = "WARS_SEX";
        public const string WarsAge = "WARS_AGE";
        public const string WrkRptMaintType = "WRK_RPT_MAINT_TYPE";
        public const string VolumeRange = "VOLUME_RANGE1";
        public const string WarsRptRecordType = "WARS_RPT_RECORD_TYPE";
        public const string WarsRptSign = "WARS_RPT_SIGN";
        public const string RkflRptRecordType = "RKFL_RPT_RECORD_TYPE";
        public const string FeatureType = "FEATURE_TYPE";
        public const string NonSpHighwayUnique = "NONSP_HIGHWAY_UNIQUE";
        public const string ThresholdSp = "THRSHLD_SP_VAR";
        public const string ValidatorProportion = "VALIDATOR_PROPORTION";

    }

    public static class ThresholdSpLevels
    {
        public const string Level1 = "Level 1";
        public const string Level2 = "Level 2";
        public const string Level3 = "Level 3";
        public const string Level4 = "Level 4";
    }

    public static class FileError
    {
        public const string ReferToRowErrors = "{ \"fieldMessages\": [ { \"field\": \"File\", \"messages\": [ \"Some headers or values are missing/incorrect. Please refer to row error(s).\" ] } ] }";
        public const string UnknownException = "{ \"fieldMessages\": [ { \"field\": \"File\", \"messages\": [ \"Encountered unexpected error. Please try again later. If it keeps happening, please contact the administrator.\" ] } ] }";
    }

    public static class RowWarning
    {
        public const string VarianceWarning = "{{ \"fieldMessages\": [ {{ \"field\": \"Variance\", \"messages\": [ \"{0} {1} is is not on the Highway Unique [{2}] within the warning threshold [{3}] metres\" ] }} ] }}";
    }

    public static class HmcrEnvironments
    {
        public const string Dev = "DEV";
        public const string Test = "TST";
        public const string Train = "TRN";
        public const string Uat = "UAT";
        public const string Prod = "PRD";
        public const string Unknown = "UNKNOWN";
    }

    public static class DotNetEnvironments
    {
        public const string Dev = "DEVELOPMENT";
        public const string Test = "STAGING";
        public const string Train = "TRAINING";
        public const string Uat = "UAT";
        public const string Prod = "PRODUCTION";
        public const string Unknown = "UNKNOWN";
    }

    public static class FeatureType
    {
        public const string None = "None";
        public const string Point = "Point";
        public const string Line = "Line";
        public const string PointLine = "Point/Line";
    }

    public static class ActivityRuleType
    {
        public const string RoadLength = "ROAD_LENGTH";
        public const string SurfaceType = "SURFACE_TYPE";
        public const string RoadClass = "ROAD_CLASS";
    }

    public static class SurfaceTypeRules
    {
        public const string NA = "NOT_APPLICABLE";
        public const string PavedStructure = "GPS_PAVED_STRUCTURE";
        public const string PavedSurface = "GPS_PAVED_SURFACE";
        public const string NonPavedSurface = "GPS_NON_PAVED_SURFACE";
        public const string Unconstructed = "GPS_NOT_UNCONSTRUCTED";
    }

    public static class MaintenanceClassRules
    {
        public const string NA = "NOT_APPLICABLE";
        public const string Class8OrF = "CLASS_8_OR_F";
        public const string NotClass8OrF = "NOT_CLASS_8_OR_F";
    }

    public static class RoadLengthRules
    {
        public const string NA = "NOT_APPLICABLE";
        public const string RATE_LANE_KM_TONNES1 = "RATE_LANE_KM_TONNES1";
        public const string RATE_LANE_KM_TONNES2 = "RATE_LANE_KM_TONNES2";
        public const string RATE_LANE_KM_LITRES1 = "RATE_LANE_KM_LITRES1";
        public const string RATE_LANE_KM_35 = "RATE_LANE_KM_35";
        public const string RATE_LANE_KM_60 = "RATE_LANE_KM_60";
        public const string LANE_METERS_35 = "LANE_METERS_35";
        public const string LANE_KM = "LANE_KM";
        public const string LANE_KM_20 = "LANE_KM_20";
        public const string LANE_METERS = "LANE_METERS";
        public const string ROAD_KM = "ROAD_KM";
        public const string ROAD_KM_20 = "ROAD_KM_20";
        public const string ROAD_METERS = "ROAD_METERS";
        public const string ROAD_METERS_20 = "ROAD_METERS_20";
        public const string GUARDRAIL_LEN_METERS = "GUARDRAIL_LEN_METERS";
    }
    
    public static class RoadSurface
    {
        public const string HOT_MIX = "1";
        public const string COLD_MIX = "2";
        public const string CONCRETE = "3";
        public const string SURFACE_TREATED = "4";
        public const string GRAVEL = "5";
        public const string DIRT = "6";
        public const string CLEARED = "E";
        public const string UNCLEARED = "F";
        public const string OTHER = "7";
        public const string UNKNOWN = "Z";
    }

    public static class ValidatorProportionCode
    {
        public const string SURFACE_TYPE_PAVED = "SURFACE_TYPE_PAVED";
        public const string SURFACE_TYPE_UNPAVED = "SURFACE_TYPE_UNPAVED";
        public const string SURFACE_TYPE_UNCONSTRUCTED = "SURFACE_TYPE_UNCNSTR";
        public const string MAINTENANCE_CLASS = "MAINTENANCE_CLASS";
        public const string STRUCTURE_VARIANCE_M = "STRUCTURE_VARIANCE_M";
    }

    public static class StructureType
    {
        public const string BRIDGE = "BRIDGE";
        public const string CULVERT = "CULVERT";
        public const string MARINE = "MARINE";
        public const string RWALL = "RWALL";
        public const string SIGN = "SIGN";
        public const string TUNNEL = "TUNNEL";

    }

    /// <summary>
    /// Spatial Data
    /// None - Non-Location specific reporting Fields
    /// GPS - Location specific reporting fields(GPS)
    /// LRS - Location specific reporting (without GPS)
    /// </summary>
    public enum SpatialData
    {
        None,
        Gps,
        Lrs
    }

    public static class DitchVolume
    {
        public const string Threshold = ">5.0";
    }

    public enum SpValidationResult
    {
        Success, Fail, NonSpatial
    }

    public static class GpsCoords
    {
        public const decimal MaxLongitude = -109;
        public const decimal MinLongitude = -141;
        public const decimal MaxLatitude = 62;
        public const decimal MinLatitude = 47;
    }

    public static class KeycloakMapperConfig
    {
        public const string DefaultProtocol = "openid-connect";
        public const string OidcAudienceMapper = "oidc-audience-mapper";
        public const string OidcHardcodedClaimMapper = "oidc-hardcoded-claim-mapper";
        public const string IncludedClientAudience = "included.client.audience";
        public const string IncludedCustomAudience = "included.custom.audience";
        public const string AccessTokenClaim = "access.token.claim";
        public const string ClaimName = "claim.name";
        public const string ClaimValue = "claim.value";
        public const string JsonTypeLabel = "jsonType.label";
        public const string ApiClient = "api_client";
    }
}
