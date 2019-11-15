using Hmcr.Api.Extensions;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Hmcr.Api.Authentication
{
    public class SmAuthenticationHandler : AuthenticationHandler<SmAuthenticationOptions>
    {
        private IUserService _userService;
        private HmcrCurrentUser _curentUser;
        private HttpContext _context;

        public SmAuthenticationHandler(
            IUserService userService,
            HmcrCurrentUser currentUser, 
            IOptionsMonitor<SmAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _userService = userService;
            _curentUser = currentUser;            
        }


        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            _context = Request.HttpContext;

            var userGuid = _context.Request.Headers[HmcrClaimTypes.UserGuid].FirstOrDefault();

            if (userGuid.IsEmpty())
            {
                return AuthenticateResult.Fail("Access Denied");
            }

            ReadSmHeaders();

            var userExists = await _userService.ProcessFirstUserLoginAsync();

            if (!userExists)
            {
                return AuthenticateResult.Fail($"Access Denied - User[{_curentUser.UniversalId}] does not exist");
            }

            var claims = GetClaims();

            _curentUser.UserInfo = await _userService.GetCurrentUserAsync();

            AddClaimsFromUserInfo(claims, _curentUser.UserInfo);

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, SmAuthenticationOptions.Scheme));            
            return AuthenticateResult.Success(new AuthenticationTicket(principal, null, SmAuthenticationOptions.Scheme));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Context.Response.ContentType = "application/json";
            Context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var problem = new ValidationProblemDetails()
            {
                Type = "https://hmcr.bc.gov.ca/exception",
                Title = "Access denied",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Authentication failed.",
                Instance = Context.Request.Path
            };

            problem.Extensions.Add("traceId", Context.TraceIdentifier);

            return Context.Response.WriteJsonAsync(problem, "application/problem+json");
        }

        private void ReadSmHeaders()
        {
            _curentUser.UserGuid = new Guid(_context.Request.Headers[HmcrClaimTypes.UserGuid].FirstOrDefault());
            _curentUser.UserType = _context.Request.Headers[HmcrClaimTypes.UserType].FirstOrDefault().ToUpperInvariant();
            _curentUser.UniversalId = _context.Request.Headers[HmcrClaimTypes.UniversalId].FirstOrDefault();

            var bizGuid = _context.Request.Headers[HmcrClaimTypes.BusinessGuid].FirstOrDefault();
            _curentUser.BusinessGuid = bizGuid.IsEmpty() ? (Guid?)null : new Guid(bizGuid);

            _curentUser.AuthDirName = _context.Request.Headers[HmcrClaimTypes.AuthDirName].FirstOrDefault().ToUpperInvariant();
            _curentUser.Email = _context.Request.Headers[HmcrClaimTypes.Email].FirstOrDefault();
            _curentUser.UserName = _context.Request.Headers[HmcrClaimTypes.UserName].FirstOrDefault();
            _curentUser.BusinessLegalName = _context.Request.Headers[HmcrClaimTypes.BusinessLegalName].FirstOrDefault();
            _curentUser.BusinessNumber = _context.Request.Headers[HmcrClaimTypes.BusinessNumber].FirstOrDefault();
        }

        private List<Claim> GetClaims()
        {
            var claims = new List<Claim>();

            if (_curentUser.UniversalId.IsNotEmpty())
            {
                claims.Add(new Claim(HmcrClaimTypes.UniversalId, _curentUser.UniversalId));
                claims.Add(new Claim(ClaimTypes.Name, _curentUser.UniversalId)); //important: it's the username in HMR_SYSTEM_USER
            }

            if (_curentUser.UserGuid != null) claims.Add(new Claim(HmcrClaimTypes.UserGuid, _curentUser.UserGuid.ToString()));
            if (_curentUser.UserType.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.UserType, _curentUser.UserType));
            if (_curentUser.BusinessGuid != null) claims.Add(new Claim(HmcrClaimTypes.BusinessGuid, _curentUser.BusinessGuid.ToString()));
            if (_curentUser.AuthDirName.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.AuthDirName, _curentUser.AuthDirName));
            if (_curentUser.Email.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.Email, _curentUser.Email));
            if (_curentUser.UserName.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.UserName, _curentUser.UserName));
            if (_curentUser.BusinessLegalName.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.BusinessLegalName, _curentUser.BusinessLegalName));
            if (_curentUser.BusinessNumber.IsNotEmpty()) claims.Add(new Claim(HmcrClaimTypes.BusinessNumber, _curentUser.BusinessNumber));

            return claims;
        }

        private void AddClaimsFromUserInfo(List<Claim> claims, UserCurrentDto user)
        {
            foreach(var permission in user.Permissions)
            {
                claims.Add(new Claim(HmcrClaimTypes.Permission, permission));
            }

            foreach(var serviceArea in user.ServiceAreas)
            {
                claims.Add(new Claim(HmcrClaimTypes.ServiceAreaNumber, serviceArea.ServiceAreaNumber.ToString()));
            }
        }
    }
}