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
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string Regex { get; set; }
        public string LookUpCode { get; set; }

        public FieldValidationRule()
        {

        }
    }
}
