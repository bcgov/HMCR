using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrActivityCodeRule
    {
        public decimal ActivityCodeRuleId { get; set; }
        public decimal ActivityCodeId { get; set; }
        public decimal ActivityRuleId { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual HmrActivityCode ActivityCode { get; set; }
        public virtual HmrActivityRule ActivityRule { get; set; }
    }
}
