using Hmcr.Model.Dtos.District;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.Region
{
    public class RegionDto
    {
        public RegionDto()
        {
            Districts = new HashSet<DistrictDto>();
        }

        public decimal RegionId { get; set; }
        public decimal RegionNumber { get; set; }
        public string RegionName { get; set; }

        public virtual ICollection<DistrictDto> Districts { get; set; }
    }
}
