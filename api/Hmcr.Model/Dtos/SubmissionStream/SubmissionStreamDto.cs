using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.SubmissionStream
{
    public class SubmissionStreamDto
    {
        [JsonPropertyName("id")]
        public decimal SubmissionStreamId { get; set; }
        [JsonPropertyName("name")]
        public string StreamName { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? FileSizeLimit { get; set; }
        public string StagingTableName { get; set; }
        public bool IsActive => EndDate == null || EndDate > DateTime.Today;
    }
}
