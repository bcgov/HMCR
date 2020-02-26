using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrActivityCode
    {
        public decimal ActivityCodeId { get; set; }
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MaintenanceType { get; set; }
        public decimal LocationCodeId { get; set; }
        public string FeatureType { get; set; }
        public bool? IsSiteNumRequired { get; set; }
        public string ActivityApplication { get; set; }
        public DateTime? EndDate { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string AppCreateUserid { get; set; }
        public DateTime AppCreateTimestamp { get; set; }
        public Guid AppCreateUserGuid { get; set; }
        public string AppCreateUserDirectory { get; set; }
        public string AppLastUpdateUserid { get; set; }
        public DateTime AppLastUpdateTimestamp { get; set; }
        public Guid AppLastUpdateUserGuid { get; set; }
        public string AppLastUpdateUserDirectory { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual HmrLocationCode LocationCode { get; set; }
    }
}
