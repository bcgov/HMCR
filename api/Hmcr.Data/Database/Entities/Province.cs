using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class Province
    {
        public int ProvinceId { get; set; }
        public string ProvinceCode { get; set; }
        public string Description { get; set; }
        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }
    }
}
