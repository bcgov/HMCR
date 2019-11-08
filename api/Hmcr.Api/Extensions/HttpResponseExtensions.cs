using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hmcr.Api.Extensions
{
    public static class HttpResponseExtensions
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public static Task WriteJsonAsync<T>(this HttpResponse response, T obj, string contentType = null)
        {
            response.ContentType = contentType ?? "application/json";
            return response.WriteAsync(JsonSerializer.Serialize<T>(obj, _jsonOptions));
        }
    }
}
