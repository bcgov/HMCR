using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class HmrCodeTimeOfDayV
    {
        public decimal CodeLookupId { get; set; }
        public decimal? TimeOfKillCode { get; set; }
        public string TimeOfDayDescription { get; set; }
    }
}
