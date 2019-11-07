using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos
{
    public class CountryDto
    {
        public CountryDto()
        {
            Provinces = new HashSet<ProvinceDto>();
        }

        [JsonPropertyName("id")]
        public int CountryId { get; set; }
        public string CountryCode { get; set; }
        public string Description { get; set; }
        public ICollection<ProvinceDto> Provinces { get; set; }
    }
}
