using Hmcr.Api.Extensions;
using Hmcr.Bceid;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hmcr.Api.Authentication
{
    public class HmcrJwtBearerEvents : JwtBearerEvents
    {
        private IUserService _userService;
        private HmcrCurrentUser _curentUser;
        private IBceidApi _bceid;

        public HmcrJwtBearerEvents(IWebHostEnvironment env, IUserService userService,
            HmcrCurrentUser currentUser, IBceidApi bceid) : base()
        {
            _userService = userService;
            _curentUser = currentUser;
            _bceid = bceid;
        }

        public override async Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var problem = new ValidationProblemDetails()
            {
                Type = "https://hmcr.bc.gov.ca/exception",
                Title = "Access denied",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Authentication failed.",
                Instance = context.Request.Path
            };

            problem.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

            await context.Response.WriteJsonAsync(problem, "application/problem+json");
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            await PopulateCurrentUser(context.Principal);

            if (_curentUser.UserGuid == null)
            {
                context.Fail("Access Denied");
                return;
            }

            var userExists = await _userService.ProcessFirstUserLoginAsync();

            if (!userExists)
            {
                context.Fail($"Access Denied - User[{_curentUser.UniversalId}] does not exist");
                return;
            }

            _curentUser.UserInfo = await _userService.GetCurrentUserAsync();

            AddClaimsFromUserInfo(context.Principal, _curentUser.UserInfo);
        }

        private async Task PopulateCurrentUser(ClaimsPrincipal principal)
        {
            _curentUser.UserName = principal.FindFirstValue(HmcrClaimTypes.KcUsername);
            var username = _curentUser.UserName.Split("@");
            var userid = username[0].ToUpperInvariant();
            var directory = username[1].ToUpperInvariant();            
            var userType = directory.ToUpperInvariant() == "IDIR" ? UserTypeDto.INTERNAL : UserTypeDto.BUSINESS;

            var (error, account) = await _bceid.GetBceidAccountCachedAsync(userid, userType);
            if (account == null)
            {
                throw new Exception(error);
            }

            if (directory == "IDIR")
            {
                _curentUser.UserGuid = account.UserGuid;
                _curentUser.UserType = UserTypeDto.INTERNAL;
            }
            else
            {
                _curentUser.UserGuid = account.UserGuid;
                _curentUser.BusinessGuid = account.BusinessGuid;
                _curentUser.BusinessLegalName = account.BusinessLegalName;
                _curentUser.BusinessNumber = account.BusinessNumber;
                _curentUser.UserType = UserTypeDto.BUSINESS;
            }

            _curentUser.UniversalId = userid;
            _curentUser.AuthDirName = directory;
            _curentUser.Email = account.Email;
            _curentUser.UserName = userid;
            _curentUser.FirstName = account.FirstName;
            _curentUser.LastName = account.LastName;
        }

        private void AddClaimsFromUserInfo(ClaimsPrincipal principal, UserCurrentDto user)
        {
            var claims = new List<Claim>();

            foreach (var permission in user.Permissions)
            {
                claims.Add(new Claim(HmcrClaimTypes.Permission, permission));
            }

            foreach (var serviceArea in user.ServiceAreas)
            {
                claims.Add(new Claim(HmcrClaimTypes.ServiceAreaNumber, serviceArea.ServiceAreaNumber.ToString()));
            }

            claims.Add(new Claim(ClaimTypes.Name, _curentUser.UniversalId));

            principal.AddIdentity(new ClaimsIdentity(claims));
        }
    }
}
