using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hmcr.Chris
{
    public interface IExportApi
    {
        Task<HttpResponseMessage> ExportReport(string query);
    }
    public class ExportApi : IExportApi
    {
        private HttpClient _client;
        private IApi _api;
        private string _path;

        public ExportApi(HttpClient client, IApi api, IConfiguration config)
        {
            _client = client;
            _api = api;
            _path = config.GetValue<string>("CHRIS:ExportPath");
        }

        public async Task<HttpResponseMessage> ExportReport(string query)
        {
            return await _api.Get(_client, _path + "&" + query);
        }
    }
}
