using System;

namespace Hmcr.Model.Dtos.RockfallReport
{
    public class RockfallRptInitCsvDto
    {
        public string ServiceArea { get; set; }
        public string RecordType { get; set; }
        public string McrrIncidentNumber { get; set; }
        public DateTime? ReportDate { get; set; }
    }
}
