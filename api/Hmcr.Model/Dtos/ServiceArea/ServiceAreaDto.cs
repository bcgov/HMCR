using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos.ServiceArea
{
    public class ServiceAreaDto
    {
        public decimal ServiceAreaId { get; set; }
        [JsonPropertyName("id")]
        public decimal ServiceAreaNumber { get; set; }
        public string ServiceAreaName { get; set; }
        public decimal DistrictNumber { get; set; }
        public string Name => $"{ServiceAreaNumber} {ServiceAreaName}";
    }
}
