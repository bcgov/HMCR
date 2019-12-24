using Hmcr.Model.Dtos.MimeType;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.SubmissionStatus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionObjectDto
    {
        [JsonPropertyName("id")]
        public decimal SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public decimal SubmissionStatusId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal SubmissionStreamId { get; set; }
        public decimal? PartyId { get; set; }
        public string ErrorDetail { get; set; }
        public string FileHash { get; set; }
    }
}
