using System;
using System.Collections.Generic;

namespace Hmcr.Data.Database.Entities
{
    public partial class Country
    {
        public Country()
        {
            Provinces = new HashSet<Province>();
        }

        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Province> Provinces { get; set; }
    }
}
