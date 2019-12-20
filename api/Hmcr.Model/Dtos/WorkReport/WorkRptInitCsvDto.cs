using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkRptInitCsvDto
    {
        public string ServiceArea { get; set; }
        public string RecordNumber { get; set; }
        public DateTime EndDate { get; set; }
    }
}
