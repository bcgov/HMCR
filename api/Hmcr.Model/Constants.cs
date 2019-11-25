using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model
{
    public static class Constants
    {
        public static DateTime MaxDate = new DateTime(9999, 12, 31);
    }

    public static class Permissions
    {
        public const string CodeReadWrite = "CODE_RW";
        public const string CodeRead = "CODE_R";
        public const string UserReadWrite = "USER_RW";
        public const string UserRead = "USER_R";
        public const string RoleReadWrite = "ROLE_RW";
        public const string RoleRead = "ROLE_R";
        public const string FileUploadReadWrite = "FILE_RW";
        public const string FileUploadRead = "FILE_R";
    }

    public static class Entities
    {
        public const string User = "user";
    }

    public static class FieldTypes
    {
        public const string String = "S";
        public const string Decimal = "N";
        public const string Date = "D";
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
    }
}
