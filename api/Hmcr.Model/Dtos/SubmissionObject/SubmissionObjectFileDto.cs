using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionObjectFileDto
    {
        [JsonPropertyName("id")]
        public decimal SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public byte[] DigitalRepresentation { get; set; }
        public string MimeTypeCode { get; set; }
    }
}
