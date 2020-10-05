using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrActivityRule
    {
        public HmrActivityRule()
        {
            HmrActivityCodeRules = new HashSet<HmrActivityCodeRule>();
        }

        public decimal ActivityRuleId { get; set; }
        public string ActivityRuleSet { get; set; }
        public string ActivityRuleName { get; set; }
        public string ActivityRuleExecName { get; set; }
        public decimal? DisplayOrder { get; set; }
        public DateTime? EndDate { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }

        public virtual ICollection<HmrActivityCodeRule> HmrActivityCodeRules { get; set; }
    }
}
