using System;

namespace Hmcr.Model.Dtos.RockfallReport
{
    public class RockfallRptInitCsvDto : IRptInitCsvDto
    {
        public int RowNum { get; set; }

        public string ServiceArea { get; set; }
        public string McrrIncidentNumber { get; set; }
        public DateTime? ReportDate { get; set; }
    }
}
