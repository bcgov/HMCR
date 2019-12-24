using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkRptUntypedCsvDto
    {
        public decimal RowId { get; set; }
        public string RecordType { get; set; }
        public string ServiceArea { get; set; }
        public string RecordNumber { get; set; }
        public string TaskNumber { get; set; }
        public string ActivityNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Accomplishment { get; set; }
        public string UnitOfMeasure { get; set; }
        public string PostedDate { get; set; }
        public string HighwayUnique { get; set; }
        public string Landmark { get; set; }
        public string StartOffset { get; set; }
        public string EndOffset { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string EndLatitude { get; set; }
        public string EndLongitude { get; set; }
        public string StructureNumber { get; set; }
        public string SiteNumber { get; set; }
        public string ValueOfWork { get; set; }
        public string Comments { get; set; }
    }
}
