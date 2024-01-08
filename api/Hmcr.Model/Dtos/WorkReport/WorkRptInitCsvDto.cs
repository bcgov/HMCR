using System;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkRptInitCsvDto : IRptInitCsvDto
    {
        public int RowNum { get; set; }
        public string ServiceArea { get; set; }
        public string RecordNumber { get; set; }
        public DateTime? EndDate { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string EndLatitude { get; set; }
        public string EndLongitude { get; set; }
    }
}
