using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrAllValidationRulesV
    {
        public string ValidationSource { get; set; }
        public decimal Id { get; set; }
        public decimal? SubmissionStreamId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ColumnName { get; set; }
        public string EntityName { get; set; }
        public bool? IsRequired { get; set; }
        public decimal? MaxLength { get; set; }
        public decimal? MinLength { get; set; }
        public decimal? MaxValue { get; set; }
        public decimal? MinValue { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? MinDate { get; set; }
        public string RegExp { get; set; }
        public string CodeSet { get; set; }
        public DateTime? ElementAttributeEndDate { get; set; }
        public DateTime? StreamEndDate { get; set; }
    }
}
