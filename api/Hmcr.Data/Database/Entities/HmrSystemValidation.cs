using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrSystemValidation
    {
        public decimal SystemValidationId { get; set; }
        public string EntityName { get; set; }
        public string AttributeName { get; set; }
        public string AttributeType { get; set; }
        public bool? IsRequired { get; set; }
        public decimal? MaxLength { get; set; }
        public decimal? MinLength { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinValue { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? MinDate { get; set; }
        public string RegExp { get; set; }
        public string CodeSet { get; set; }
        public DateTime? EndDate { get; set; }
        public long ConcurrencyControlNumber { get; set; }
        public string DbAuditCreateUserid { get; set; }
        public DateTime DbAuditCreateTimestamp { get; set; }
        public string DbAuditLastUpdateUserid { get; set; }
        public DateTime DbAuditLastUpdateTimestamp { get; set; }
    }
}
