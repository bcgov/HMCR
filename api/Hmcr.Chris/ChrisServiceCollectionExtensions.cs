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
            services.AddHttpClient<MapsService>(client =>
            {
                client.BaseAddress = new Uri(config.GetSection("ChrisUris:MapsUri").Value);
                client.Timeout = new TimeSpan(0, 0, 15);
                client.DefaultRequestHeaders.Clear();
            });

            services.AddHttpClient<OasService>(client =>
            {
                client.BaseAddress = new Uri(config.GetSection("ChrisUris:OasUri").Value);
                client.Timeout = new TimeSpan(0, 0, 15);
                client.DefaultRequestHeaders.Clear();

                var userId = config.GetSection("ServiceAccount:UserId").Value;
                var password = config.GetSection("ServiceAccount:Password").Value;
                var basicAuth = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{userId}:{password}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuth);
            });

            services.AddScoped<IApi, Api>();
        }
    }
}
