using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrCodeLookup
    {
        public decimal CodeLookupId { get; set; }
        public string CodeSet { get; set; }
        public string CodeName { get; set; }
        public string CodeValueText { get; set; }
        public decimal? CodeValueNum { get; set; }
        public string CodeValueFormat { get; set; }
        public decimal? DisplayOrder { get; set; }
        public DateTime? EndDate { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }
        public bool IsIntegerOnly { get; set; }
    }
}
