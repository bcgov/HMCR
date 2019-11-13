using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrRegion
    {
        public HmrRegion()
        {
            HmrDistricts = new HashSet<HmrDistrict>();
        }

        public decimal RegionId { get; set; }
        public decimal RegionNumber { get; set; }
        public string RegionName { get; set; }
        public decimal ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual ICollection<HmrDistrict> HmrDistricts { get; set; }
    }
}
