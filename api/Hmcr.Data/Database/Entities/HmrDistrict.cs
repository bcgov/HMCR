using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrDistrict
    {
        public HmrDistrict()
        {
            HmrServiceAreas = new HashSet<HmrServiceArea>();
        }

        public decimal DistrictId { get; set; }
        public decimal DistrictNumber { get; set; }
        public string DistrictName { get; set; }
        public decimal RegionNumber { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual HmrRegion RegionNumberNavigation { get; set; }
        public virtual ICollection<HmrServiceArea> HmrServiceAreas { get; set; }
    }
}
