using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Hmcr.Chris
{
    public static class ChrisServiceCollectionExtensions
    {
        public static void AddChrisHttpClient(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IMapsApi, MapsApi>(client =>
            {
                client.BaseAddress = new Uri(config.GetValue<string>("CHRIS:MapUrl"));
                client.Timeout = new TimeSpan(0, 0, config.GetValue<int>("Timeouts:MapsAPI"));
                client.DefaultRequestHeaders.Clear();
            });

            services.AddHttpClient<IOasApi, OasApi>(client =>
            {
                client.BaseAddress = new Uri(config.GetValue<string>("CHRIS:OASUrl"));
                client.Timeout = new TimeSpan(0, 0, config.GetValue<int>("Timeouts:OasAPI"));
                client.DefaultRequestHeaders.Clear();

                var userId = config.GetValue<string>("ServiceAccount:User");
                var password = config.GetValue<string>("ServiceAccount:Password");
                var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{userId}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            });

            services.AddHttpClient<IExportApi, ExportApi>(client =>
            {
                client.BaseAddress = new Uri(config.GetValue<string>("CHRIS:ExportUrl"));
                client.Timeout = new TimeSpan(0, 0, config.GetValue<int>("Timeouts:ExportAPI"));
                client.DefaultRequestHeaders.Clear();

                var userId = config.GetValue<string>("ServiceAccount:User");
                var password = config.GetValue<string>("ServiceAccount:Password");
                var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{userId}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            });

            services.AddHttpClient<IInventoryApi, InventoryApi>(client =>
            {
                client.BaseAddress = new Uri(config.GetValue<string>("CHRIS:OASUrl"));
                //TODO: need to set the timeouts to be configurable with the ConfigMap of OCP
                client.Timeout = new TimeSpan(0, 0, config.GetValue<int>("Timeouts:InventoryAPI"));
                client.DefaultRequestHeaders.Clear();

                var userId = config.GetValue<string>("ServiceAccount:User");
                var password = config.GetValue<string>("ServiceAccount:Password");
                var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{userId}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            });

            services.AddScoped<IApi, Api>();
        }
    }
}
