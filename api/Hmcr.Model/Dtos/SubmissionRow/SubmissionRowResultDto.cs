namespace Hmcr.Model.Dtos.SubmissionRow
{
    public class SubmissionRowResultDto
    {
        public decimal RowId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal? RowStatusId { get; set; }
        public string Description { get; set; }
        public string RecordNumber { get; set; }
        public string RowValue { get; set; }
        public string ErrorDetail { get; set; }
        public string WarningDetail { get; set; }
        public decimal? RowNum { get; set; }
        public decimal? StartVariance { get; set; }
        public decimal? EndVariance { get; set; }

        public bool IsResubmitted { get; set; }
    }
}
