using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class ActivityCodeCreateDto
    {
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MaintenanceType { get; set; }
        public decimal LocationCodeId { get; set; }
        public string PointLineFeature { get; set; }
        public string ActivityApplication { get; set; }
        public DateTime? EndDate { get; set; }

        public long ConcurrencyControlNumber => 1;
        /*CONCURRENCY_CONTROL_NUMBER ??*/
    }
}
