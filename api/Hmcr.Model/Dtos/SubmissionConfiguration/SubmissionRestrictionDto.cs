namespace Hmcr.Model.Dtos.SubmissionConfiguration
{
    public class SubmissionRestrictionRowDto
    {
        public int RowNum { get; set; }
        public string RecordNumber { get; set; }
        public string ActivityNumber { get; set; }
        public decimal? Accomplishment { get; set; }
    }

    public class SubmissionRestrictionViolationDto
    {
        public decimal SubmissionConfigurationId { get; set; }
        public string ConfigurationKey { get; set; }
        public int RowNum { get; set; }
        public string RecordNumber { get; set; }
        public string ActivityNumber { get; set; }
        public string Message { get; set; }
    }
}
