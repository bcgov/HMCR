using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrActivityCodeRule
    {
        public HmrActivityCodeRule()
        {
            HmrActivityCodeRoadClassRuleNavigations = new HashSet<HmrActivityCode>();
            HmrActivityCodeRoadLengthRuleNavigations = new HashSet<HmrActivityCode>();
            HmrActivityCodeSurfaceTypeRuleNavigations = new HashSet<HmrActivityCode>();
        }

        public decimal ActivityCodeRuleId { get; set; }
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

        public virtual ICollection<HmrActivityCode> HmrActivityCodeRoadClassRuleNavigations { get; set; }
        public virtual ICollection<HmrActivityCode> HmrActivityCodeRoadLengthRuleNavigations { get; set; }
        public virtual ICollection<HmrActivityCode> HmrActivityCodeSurfaceTypeRuleNavigations { get; set; }
    }
}
