using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model
{
    public static class Constants
    {
        public static DateTime MaxDate = new DateTime(9999, 12, 31);
        public static decimal MaxFileSize = 2097152;
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
    }

    public static class Entities
    {
        public const string User = "user";
        public const string Role = "role";

        public const string WorkReportD2 = "workd2";
        public const string WorkReportD2B = "workd2b";
        public const string WorkReportD3 = "workd3";
        public const string WorkReportD3Site = "workrd3site";
        public const string WorkReportD4 = "workd4";
        public const string WorkReportD4Site = "workd4site";

        public const string RockfallReport = "rockfall";
        public const string RockfallReportGps = "rockfallgps";
        public const string RockfallReportLrs = "rockfalllrs";

        public const string WildlifeReport = "wildlife";
        public const string WildlifeReportGps = "wildlifegps";
        public const string WildlifeReportLrs = "wildlifelrs";
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
    }

    public static class StatusType
    {
        public const string File = "F";
        public const string Row = "R";
    }

    public static class RowStatus
    {
        public const string RowReceived = "RR";
        public const string DuplicateRow = "DR";
        public const string RowError = "RE";
        public const string Success = "RS";
    }

    public static class FileStatus
    {
        public const string FileReceived = "FR";
        public const string FileError = "FE";
        public const string DuplicateSubmission = "DS";
        public const string InProgress = "DP";
        public const string DataError = "DE";
        public const string Success = "VS";
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
        public const string HighwayUniqueNumber = "HighwayUniqueNumber";
        public const string HighwayUniqueName = "HighwayUniqueName";
        public const string LandMarkName = "LandMarkName";
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
        public const string OtherVolume = "OtherVolume";

        public const string AccidentDate = "AccidentDate";
        public const string TimeOfKill = "TimeOfKill";
        public const string Latitude = "Latitude";
        public const string Longitude = "Longitude";
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
            Fields.StartLatitude, Fields.StartLongitude, Fields.HighwayUniqueNumber, Fields.HighwayUniqueName, Fields.Landmark,
            Fields.LandMarkName, Fields.StartOffset, Fields.DirectionFromLandmark, Fields.DitchVolume, Fields.TravelledLanesVolume,
            Fields.HeavyPrecip, Fields.FreezeThaw, Fields.DitchSnowIce, Fields.VehicleDamage, Fields.Name,
            Fields.McPhoneNumber, Fields.McName, Fields.ReportDate
        };
        public string[] CommonMandatoryFields => MandatoryFields;
    }

    public class WildlifeReportHeaders : IReportHeaders
    {
        public static string[] MandatoryFields = new string[]
        {
            Fields.RecordType, Fields.ServiceArea, Fields.AccidentDate, Fields.TimeOfKill, 
            Fields.WildlifeSign, Fields.Quantity, Fields.Species, Fields.Sex, Fields.Age
        };
        public string[] CommonMandatoryFields => MandatoryFields;
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
    }

    public static class FileError
    {
        public const string ReferToRowErrors = "{ \"fieldMessages\": [ { \"field\": \"File\", \"messages\": [ \"Please refer to row error(s)\" ] } ] }";
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

    public static class PointLineFeature
    {
        public const string None = "None";
        public const string Point = "Point";
        public const string Line = "Line";
        public const string Either = "Either";
    }

    /// <summary>
    /// Work Report Row Types
    /// D2 - Non-Location specific reporting Fields
    /// D3 - Location specific reporting fields(GPS)
    /// D4 - Location specific reporting(without GPS)
    /// </summary>
    public enum RowTypes
    {
        D2, D3, D4
    }
}
