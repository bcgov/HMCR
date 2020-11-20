using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Hmcr.Model.Utils;
namespace Hmcr.Model.Dtos.User
{
    public class UserSearchExportDto
    {
        [JsonPropertyName("id")]
        public decimal SystemUserId { get; set; }
        public string UserType { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Username { get; set; }
        public string BusinessLegalName { get; set; }
        public string Email { get; set; }
        public string ServiceAreas { get; set; }
        public string UserRoles { get; set; }
        public bool HasLogInHistory { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive => EndDate == null || EndDate > DateTime.Today;
        public string ToCsv()
        {
            var wholeNumberFields = new string[] { Fields.Username, Fields.UserType };
            return CsvUtils.ConvertToCsv<UserSearchExportDto>(this, wholeNumberFields);
        }
    }
}
