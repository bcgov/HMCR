using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class LocationCodeDto
    {
        public decimal LocationCodeId { get; set; }
        public string LocationCode { get; set; }
        public string RequiredLocationDetails { get; set; }
        public string AdditionalInfo { get; set; }
        public string ReportingFrequency { get; set; }

    }
}
