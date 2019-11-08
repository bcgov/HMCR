using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hmcr.Model.Dtos
{
    public class ProvinceDto
    {
        [JsonPropertyName("id")]
        public int ProvinceId { get; set; }
        public string ProvinceCode { get; set; }
        [Required]
        public string Description { get; set; }
        public int CountryId { get; set; }
    }
}
