using Microsoft.AspNetCore.Authentication;

namespace Hmcr.Api.Authentication
{
    public class SmAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string Scheme = "site-minder-auth";
    }
}