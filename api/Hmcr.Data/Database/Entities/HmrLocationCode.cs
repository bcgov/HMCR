using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrLocationCode
    {
        public HmrLocationCode()
        {
            HmrActivityCodes = new HashSet<HmrActivityCode>();
        }

        public decimal LocationCodeId { get; set; }
        public string LocationCode { get; set; }
        public string RequiredLocationDetails { get; set; }
        public string AdditionalInfo { get; set; }
        public string ReportingFrequency { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual ICollection<HmrActivityCode> HmrActivityCodes { get; set; }
    }
}
