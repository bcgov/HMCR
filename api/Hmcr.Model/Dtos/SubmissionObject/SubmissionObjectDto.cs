using Hmcr.Model.Dtos.MimeType;
using Hmcr.Model.Dtos.Party;
using Hmcr.Model.Dtos.ServiceArea;
using Hmcr.Model.Dtos.SubmissionStatus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.SubmissionObject
{
    public class SubmissionObjectDto
    {
        public decimal SubmissionObjectId { get; set; }
        public string FileName { get; set; }
        public byte[] DigitalRepresentation { get; set; }
        public decimal MimeTypeId { get; set; }
        public decimal SubmissionStatusId { get; set; }
        public decimal ServiceAreaNumber { get; set; }
        public decimal PartyId { get; set; }
    }
}
