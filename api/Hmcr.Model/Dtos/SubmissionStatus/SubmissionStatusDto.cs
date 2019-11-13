using Hmcr.Model.Dtos.SubmissionObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.SubmissionStatus
{
    public class SubmissionStatusDto
    {
        public decimal SubmissionStatusId { get; set; }
        public string SubmissionStatusCode { get; set; }
        public string Description { get; set; }
    }
}
