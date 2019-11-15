using Hmcr.Model.Dtos.Region;
using Hmcr.Model.Dtos.ServiceArea;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.District
{
    public class DistrictDto
    {
        public decimal DistrictId { get; set; }
        public decimal DistrictNumber { get; set; }
        public string DistrictName { get; set; }
        public decimal RegionNumber { get; set; }
    }
}
