using Hmcr.Model;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Hmcr.Api.Authentication
{
    public class SmAuthenticationHandler : AuthenticationHandler<SmAuthenticationOptions>
    {
        private SmHeaders _smHeaders;
        private HttpContext _context;

        public SmAuthenticationHandler(
            SmHeaders smHeaders, 
            IOptionsMonitor<SmAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _smHeaders = smHeaders;
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _context = Request.HttpContext;

            ReadSmHeaders();

            if (string.IsNullOrWhiteSpace(_smHeaders.UniversalId))
            {
                return AuthenticateResult.Fail("Access Denied");
            }

            //Database look up and update if necessary
            //  Find User with UserGuid first and if not found, find with UniversalId
            //  Update Email and other fields if they are different
            //  Make sure company exists if BusinessGuid is present
            //  Update company if necessary

            await Task.CompletedTask; //Remove after DB access

            var principal = new ClaimsPrincipal(new ClaimsIdentity(GetClaims(), SmAuthenticationOptions.Scheme));            
            return AuthenticateResult.Success(new AuthenticationTicket(principal, null, SmAuthenticationOptions.Scheme));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            return Context.Response.WriteAsync(new Error()
            {
                ErrorCode = Context.Response.StatusCode,
                Message = "Authentication failed."
            }.ToString());
        }

        private void ReadSmHeaders()
        {
            _smHeaders.UserGuid = _context.Request.Headers[HmcrClaimTypes.UserGuid].FirstOrDefault();
            _smHeaders.UserType = _context.Request.Headers[HmcrClaimTypes.UserType].FirstOrDefault();
            _smHeaders.UniversalId = _context.Request.Headers[HmcrClaimTypes.UniversalId].FirstOrDefault();
            _smHeaders.BusinessGuid = _context.Request.Headers[HmcrClaimTypes.BusinessGuid].FirstOrDefault();
            _smHeaders.AuthDirName = _context.Request.Headers[HmcrClaimTypes.AuthDirName].FirstOrDefault();
            _smHeaders.Email = _context.Request.Headers[HmcrClaimTypes.Email].FirstOrDefault();
            _smHeaders.UserName = _context.Request.Headers[HmcrClaimTypes.UserName].FirstOrDefault();
            _smHeaders.BusinessLegalName = _context.Request.Headers[HmcrClaimTypes.BusinessLegalName].FirstOrDefault();
            _smHeaders.BusinessNumber = _context.Request.Headers[HmcrClaimTypes.BusinessNumber].FirstOrDefault();
        }

        private List<Claim> GetClaims()
        {
            var claims = new List<Claim>();

            if (_smHeaders.UniversalId.IsNotEmpty())
            {
                claims.Add(new Claim(HmcrClaimTypes.UniversalId, _smHeaders.UniversalId));
                claims.Add(new Claim(ClaimTypes.Name, _smHeaders.UniversalId));
            }

            if (_smHeaders.UserGuid.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.UserGuid, _smHeaders.UserGuid));
            if (_smHeaders.UserType.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.UserType, _smHeaders.UserType));
            if (_smHeaders.BusinessGuid.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.BusinessGuid, _smHeaders.BusinessGuid));
            if (_smHeaders.AuthDirName.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.AuthDirName, _smHeaders.AuthDirName));
            if (_smHeaders.Email.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.Email, _smHeaders.Email));
            if (_smHeaders.UserName.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.UserName, _smHeaders.UserName));
            if (_smHeaders.BusinessLegalName.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.BusinessLegalName, _smHeaders.BusinessLegalName));
            if (_smHeaders.BusinessNumber.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.BusinessNumber, _smHeaders.BusinessNumber));

            return claims;
        }
    }
}