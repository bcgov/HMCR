using Microsoft.Extensions.DependencyInjection;
using BceidService;
using Microsoft.Extensions.Configuration;
using static BceidService.BCeIDServiceSoapClient;
using System.ServiceModel;

namespace Hmcr.Bceid
{
    public static class BceidServiceCollectionExtensions
    {
        public static void AddBceidSoapClient(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<BCeIDServiceSoapClient>(provider =>
            {
                var username = config.GetValue<string>("BCEID_USER");
                var password = config.GetValue<string>("BCEID_PASSWORD");
                var url = config.GetValue<string>("BCEID_URL");
                var osid = config.GetValue<string>("BCEID_OSID");
                var guid = config.GetValue<string>("BCEID_GUID");
                var cacheLifeSpan = config.GetValue<int>("BCEID_CACHE_LIFESPAN");

                var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;

                var client = new BCeIDServiceSoapClient(EndpointConfiguration.BCeIDServiceSoap12);
                client.ClientCredentials.UserName.UserName = username;
                client.ClientCredentials.UserName.Password = password;
                client.Endpoint.Binding = binding;
                client.Endpoint.Address = new EndpointAddress(url);
                client.Osid = osid;
                client.Guid = guid;
                client.CacheLifespan = cacheLifeSpan == 0 ? 60 : cacheLifeSpan; //60 minutes default

                return client;
            });

            services.AddSingleton<IBceidApi, BceidApi>();
        }
    }
}
