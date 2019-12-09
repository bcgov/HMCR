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
    }

    public static class FieldTypes
    {
        public const string String = "S";
        public const string Decimal = "N";
        public const string Date = "D";
    }

    public static class SubmissionStreams
    {
        public const decimal WorkReport = 1;
        public const decimal RockfallReport = 2;
        public const decimal WildlifeReport = 3;

    }
    public static class TableNames
    {
        public const string WorkReport = "HMR_WORK_REPORT";
        public const string RockFallReport = "HMR_ROCKFALL_REPORT";
        public const string WildlifeReport = "HMR_WILDLIFE_REPORT";
    }

    public static class StatusType
    {
        public const string File = "F";
        public const string Row = "R";
    }

    public static class RowStatus
    {
        public const string Accepted = "A";
        public const string Duplicate = "D";
    }

    public static class FileStatus
    {
        public const string Accepted = "A";
        public const string Duplicate = "D";
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
    }


}
