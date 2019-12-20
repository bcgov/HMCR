using Hmcr.Model.Dtos.SubmissionRow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionObjectCreateDto
    {
        public SubmissionObjectCreateDto()
        {
            SubmissionRows = new List<SubmissionRowDto>();
        }

        public string FileName { get; set; }
        public byte[] DigitalRepresentation { get; set; }
        public decimal MimeTypeId { get; set; }
        public decimal SubmissionStatusId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public IList<SubmissionRowDto> SubmissionRows { get; set; }
    }
}
