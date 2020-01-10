using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.CodeLookup
{
    public class CodeLookupDto
    {
        public decimal CodeLookupId { get; set; }
        public string CodeSet { get; set; }
        public string CodeName { get; set; }
        public string CodeValueText { get; set; }
        public decimal? CodeValueNum { get; set; }
        public string CodeValueFormat { get; set; }
        public decimal? DisplayOrder { get; set; }
    }
}
