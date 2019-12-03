using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrCodeUnitOfMeasureV
    {
        public decimal CodeLookupId { get; set; }
        public string UomCode { get; set; }
        public string UomDescription { get; set; }
    }
}
