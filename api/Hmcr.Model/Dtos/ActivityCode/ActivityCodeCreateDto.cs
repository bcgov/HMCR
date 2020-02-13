using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.LocationCode
{
    public class ActivityCodeCreateDto
    {
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MaintenanceType { get; set; }
        public decimal LocationCodeId { get; set; }
        public string PointLineFeature { get; set; }
        public bool SiteNumberRequired { get; set; }
        public bool IsSiteNumRequired { get; set; }
        public DateTime? EndDate { get; set; }
    }
}