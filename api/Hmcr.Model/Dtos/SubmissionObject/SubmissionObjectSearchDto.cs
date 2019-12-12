using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionObjectSearchDto
    {
        [JsonPropertyName("id")]
        public decimal SubmissionObjectId { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public string AppCreateUserid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FileName { get; set; }
        public decimal SubmissionStreamId { get; set; }
        public string StreamName { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal SubmissionStatusId { get; set; }
        public string SubmissionStatusCode { get; set; }
        public string Description { get; set; }
        public string ErrorDetail { get; set; }
    }
}
