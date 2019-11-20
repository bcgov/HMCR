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
        public string Regex { get; set; }
        public string LookUpCode { get; set; }

        public FieldValidationRule(string entityName, string fieldName, string fieldType, bool required, int? minLength, int? maxLength, decimal? minValue, decimal? maxValue, DateTime? minDate, DateTime? maxDate, string regex, string lookUpCode)
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
            LookUpCode = lookUpCode;
        }
    }
}
