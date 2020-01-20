using Hmcr.Model.Dtos.SubmissionRow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionObjectResultDto
    {
        [JsonPropertyName("id")]
        public decimal SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public decimal SubmissionStreamId { get; set; }
        public string StreamName { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal SubmissionStatusId { get; set; }
        public string SubmissionStatusCode { get; set; }
        public string Description { get; set; }
        public string ErrorDetail { get; set; }
        public int NumResubmitRows { get { return SubmissionRows.Count(x => x.IsResubmitted == "Y"); } }
        public DateTime AppCreateTimestamp { get; set; }

        public IEnumerable<SubmissionRowResultDto> SubmissionRows { get; set; }
    }
}
