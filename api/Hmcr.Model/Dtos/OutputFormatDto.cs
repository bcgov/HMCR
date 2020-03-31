using System.Text.Json.Serialization;

namespace Hmcr.Model.Dtos
{
    public class OutputFormatDto
    {
        public const string Csv = "csv";
        public const string Json = "json";
        public const string Kml = "kml";
        public const string Gml = "gml";

        [JsonPropertyName("id")]
        public string Format { get; set; }
        [JsonPropertyName("name")]
        public string Description { get; set; }

        public static OutputFormatDto[] GetSupportedFormats()
        {
            return new OutputFormatDto[4]
            {
                new OutputFormatDto {Format = Csv, Description = "Comma Separated Values"},
                new OutputFormatDto {Format = Json, Description = "GeoJSON"},
                new OutputFormatDto {Format = Kml, Description = "KML(Keyhole Markup Language)"},
                new OutputFormatDto {Format = Gml, Description = "GML(Geography Markup Language)"}
            };
        }
    }

}
