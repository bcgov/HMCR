using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model
{
    public class FieldValidationRule
    {
        public string EntityName { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public bool Required { get; set; }
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public RegexInfo Regex { get; private set; }
        public string CodeSet { get; set; }
        public LookupItem LookupItem { get; set; }

        public FieldValidationRule(string entityName, string fieldName, string fieldType, bool required, int? minLength, int? maxLength, decimal? minValue, decimal? maxValue, DateTime? minDate, DateTime? maxDate, RegexInfo regex, string codeSet, LookupItem lookupItem = LookupItem.Value)
        {
            EntityName = entityName;
            FieldName = fieldName;
            FieldType = fieldType;
            Required = required;
            MinLength = minLength;
            MaxLength = maxLength;
            MinValue = minValue;
            MaxValue = maxValue;
            MinDate = minDate;
            MaxDate = maxDate;
            Regex = regex;
            CodeSet = codeSet;
            LookupItem = lookupItem;
        }

        public FieldValidationRule ShallowCopy(string entityName)
        {
            var rule = (FieldValidationRule)this.MemberwiseClone();
            rule.EntityName = entityName;
            return rule;
        }
    }


    public enum LookupItem
    {
        Value, Name
    }
}
