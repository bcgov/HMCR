using System;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionInfoForEmailDto
    {
        public long SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int ServiceAreaNumber { get; set; }
        public int NumOfRecords { get; set; }
        public int NumOfErrorRecords { get; set; }
        public int NumOfWarningRecords { get; set; }
        public int NumOfDuplicateRecords { get; set; }
        public int NumOfReplacedRecords { get; set; }
        public bool Success { get; set; }
    }
}
