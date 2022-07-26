using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IJsonApi
    {
        Task<HttpResponseMessage> ExportJsonReport(string query, string exportEndpointConfig);
    }
    public class JsonApi : IJsonApi
    {
        private HttpClient _client;
        private IApi _api;
        private IConfiguration _config;
        private ILogger<IExportApi> _logger;

        public JsonApi(HttpClient client, IApi api, IConfiguration config, ILogger<IExportApi> logger)
        {
            _client = client;
            _api = api;
            _config = config;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> ExportJsonReport(string query, string exportEndpointConfig)
        {
            string path = _config.GetValue<string>($"CHRIS:{exportEndpointConfig}");

            //_client.BaseAddress.ToString() + path + "&" + query
            _logger.LogInformation($"ExportReport - Calling Export URL { _client.BaseAddress.ToString() + path + "&" + query}");

            return await _api.Get(_client, path + "&" + query);
        }
    }
}
