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
        public string Name { get; set; }
        public string Description { get; set; }

        public static OutputFormatDto[] GetSupportedFormats()
        {
            return new OutputFormatDto[4]
            {
                new OutputFormatDto {Format = Csv, Name = "CSV", Description = "Comma Separated Values"},
                new OutputFormatDto {Format = Kml, Name = "KML", Description = "KML(Keyhole Markup Language)"},
                new OutputFormatDto {Format = Json, Name = "GeoJSON", Description = "GeoJSON"},
                new OutputFormatDto {Format = Gml, Name = "GML", Description = "GML(Geography Markup Language)"}
            };
        }
    }

}
