using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace Hmcr.Chris
{
    public static class ChrisServiceCollectionExtensions
    {
        /// <summary>
        /// Builds the Basic auth header for the CHRIS/OAS service account.
        /// Credentials are sent UTF-8 encoded, which the CHRIS GeoServer (Spring Security)
        /// decodes correctly. The previous ISO-8859-1 encoding silently replaced any
        /// character outside that range (e.g. an en dash in the TEST/UAT password) with
        /// '?', so the service rejected every request with 401 Unauthorized.
        /// For ASCII-only passwords the two encodings produce identical bytes.
        /// </summary>
        private static AuthenticationHeaderValue GetServiceAccountAuthHeader(IConfiguration config)
        {
            var userId = config["ServiceAccount:User"];
            var password = config["ServiceAccount:Password"];

            var nonAscii = $"{userId}{password}".Where(c => c > 127).Distinct().ToArray();
            if (nonAscii.Length > 0)
            {
                Console.Error.WriteLine(
                    "NOTICE: The ServiceAccount user/password contains non-ASCII character(s): " +
                    string.Join(", ", nonAscii.Select(c => $"U+{(int)c:X4}")) +
                    ". They are sent UTF-8 encoded, which the CHRIS GeoServer accepts. If authentication " +
                    "fails with 401, verify these characters are really part of the password and were not " +
                    "introduced by copy/paste through a word processor (e.g. a hyphen turned into an en dash).");
            }

            var basicAuth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{userId}:{password}"));
            return new AuthenticationHeaderValue("Basic", basicAuth);
        }

        public static void AddChrisHttpClient(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpClient<IMapsApi, MapsApi>(client =>
            {
                client.BaseAddress = new Uri(config["CHRIS:MapUrl"]);
                client.Timeout = new TimeSpan(0, 0, int.Parse(config["Timeouts:MapsAPI"]));
                client.DefaultRequestHeaders.Clear();
            });

            services.AddHttpClient<IOasApi, OasApi>(client =>
            {
                client.BaseAddress = new Uri(config["CHRIS:OASUrl"]);
                client.Timeout = new TimeSpan(0, 0, int.Parse(config["Timeouts:OasAPI"]));
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = GetServiceAccountAuthHeader(config);
            });

            services.AddHttpClient<IExportApi, ExportApi>(client =>
            {
                client.BaseAddress = new Uri(config["CHRIS:ExportUrl"]);
                client.Timeout = new TimeSpan(0, 0, int.Parse(config["Timeouts:ExportAPI"]));
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = GetServiceAccountAuthHeader(config);
            });

            services.AddHttpClient<IInventoryApi, InventoryApi>(client =>
            {
                client.BaseAddress = new Uri(config["CHRIS:OASUrl"]);
                //TODO: need to set the timeouts to be configurable with the ConfigMap of OCP
                client.Timeout = new TimeSpan(0, 0, int.Parse(config["Timeouts:InventoryAPI"]));
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Authorization = GetServiceAccountAuthHeader(config);
            });

            services.AddScoped<IApi, Api>();
        }
    }
}
