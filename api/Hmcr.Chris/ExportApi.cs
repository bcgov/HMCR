using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IExportApi
    {
        Task<HttpResponseMessage> ExportReport(string query, string exportEndpointConfig);
    }
    public class ExportApi : IExportApi
    {
        private HttpClient _client;
        private IApi _api;
        private IConfiguration _config;

        public ExportApi(HttpClient client, IApi api, IConfiguration config)
        {
            _client = client;
            _api = api;
            _config = config;
        }

        public async Task<HttpResponseMessage> ExportReport(string query, string exportEndpointConfig)
        {
            string path = _config.GetValue<string>($"CHRIS:{exportEndpointConfig}");
            return await _api.Get(_client, path + "&" + query);
        }
    }
}
