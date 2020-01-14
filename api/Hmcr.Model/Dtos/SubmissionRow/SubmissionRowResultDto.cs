using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
        public int RowNumber { get; set; }
    }
}
