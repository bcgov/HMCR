using System;

namespace Hmcr.Model.Dtos.Logging
{
    public class ClientLogDto
    {
        public string SupportId { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        public string ComponentStack { get; set; }
        public string Route { get; set; }
        public string ClientSessionId { get; set; }
        public string ClientTraceId { get; set; }
        public string CorrelationId { get; set; }
        public string HttpMethod { get; set; }
        public string Url { get; set; }
        public int? StatusCode { get; set; }
        public string UserAgent { get; set; }
        public string AppVersion { get; set; }
        public DateTime? TimestampUtc { get; set; }
    }
}
