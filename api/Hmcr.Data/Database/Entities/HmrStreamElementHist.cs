using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrStreamElementHist
    {
        public long StreamElementHistId { get; set; }
        public DateTime EffectiveDateHist { get; set; }
        public DateTime? EndDateHist { get; set; }
        public decimal StreamElementId { get; set; }
        public decimal SubmissionStreamId { get; set; }
        public string ElementName { get; set; }
        public string ElementType { get; set; }
        public string StagingColumnName { get; set; }
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
    }
}
