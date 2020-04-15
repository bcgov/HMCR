using System;

namespace Hmcr.Model.Dtos.WildlifeReport
{
    public class WildlifeRptInitCsvDto : IRptInitCsvDto
    {
        public int RowNum { get; set; }

        public string ServiceArea { get; set; }
        public DateTime? AccidentDate { get; set; }
    }
}
