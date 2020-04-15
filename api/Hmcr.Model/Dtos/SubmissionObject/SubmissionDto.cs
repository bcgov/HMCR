namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionDto
    {
        public decimal SubmissionObjectId { get; set; }
        public string StagingTableName { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public string FileName { get; set; }
        public string MimeTypeCode { get; set; }
        public long ConcurrencyControlNumber { get; set; }
    }
}
