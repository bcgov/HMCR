using System;

namespace Hmcr.Model.Dtos.SubmissionConfiguration
{
    public class SubmissionConfigurationNoticeDto
    {
        public decimal SubmissionConfigurationId { get; set; }
        public string ConfigurationKey { get; set; }
        public string Name { get; set; }
        public string Phase { get; set; }
        public string Severity { get; set; }
        public string Message { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
