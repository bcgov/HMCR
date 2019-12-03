using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrLocationCodeHist
    {
        public long LocationCodeHistId { get; set; }
        public DateTime EffectiveDateHist { get; set; }
        public DateTime? EndDateHist { get; set; }
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
    }
}
