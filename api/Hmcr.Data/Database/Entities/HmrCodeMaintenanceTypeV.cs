using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrCodeMaintenanceTypeV
    {
        public decimal CodeLookupId { get; set; }
        public string MaintenanceTypeCode { get; set; }
        public string MaintenanceTypeDescription { get; set; }
    }
}
