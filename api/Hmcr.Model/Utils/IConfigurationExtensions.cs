using Microsoft.Extensions.Configuration;

namespace Hmcr.Model.Utils
{
    public static class IConfigurationExtensions
    {
        public static string GetEnvironment(this IConfiguration config)
        {
            var env = config.GetValue<string>("ASPNETCORE_ENVIRONMENT").ToUpperInvariant();

            switch (env)
            {
                case DotNetEnvironments.Prod:
                    return HmcrEnvironments.Prod;
                case DotNetEnvironments.Dev:
                    return HmcrEnvironments.Dev;
                case DotNetEnvironments.Test:
                    return HmcrEnvironments.Test;
                case DotNetEnvironments.Uat:
                    return HmcrEnvironments.Uat;
                case DotNetEnvironments.Train:
                    return HmcrEnvironments.Train;
                default:
                    return HmcrEnvironments.Unknown;
            }
        }
    }
}
