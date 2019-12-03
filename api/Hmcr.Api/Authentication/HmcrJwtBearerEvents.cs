using Hmcr.Api.Extensions;
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

        public HmcrJwtBearerEvents(IWebHostEnvironment env, IUserService userService,
            HmcrCurrentUser currentUser) : base()
        {
            _userService = userService;
            _curentUser = currentUser;
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
            PopulateCurrentUser(context.Principal);

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

            await Task.CompletedTask;
        }

        private void PopulateCurrentUser(ClaimsPrincipal principal)
        {
            _curentUser.UserName = principal.FindFirstValue(HmcrClaimTypes.UserName);
            var username = _curentUser.UserName.Split("@");
            var directory = username[1];

            if (directory.ToUpperInvariant() == "IDIR")
            {
                _curentUser.UserGuid = new Guid(principal.FindFirstValue(HmcrClaimTypes.UserGuid));
                _curentUser.UserType = UserTypeDto.INTERNAL;
            }
            else
            {
                //todo: set UserGuid and BusinessGuid
                //_curentUser.UserGuid = new Guid(principal.FindFirstValue(HmcrClaimTypes.BceidGuid));
                //_curentUser.BusinessGuid = new Guid(principal.FindFirstValue(HmcrClaimTypes.BizGuid));
                //_curentUser.BusinessLegalName = principal.FindFirstValue(HmcrClaimTypes.BizLegalName);
                //_curentUser.BusinessNumber = principal.FindFirstValue(HmcrClaimTypes.BizNumber);

                _curentUser.UserType = UserTypeDto.BUSINESS;
            }

            _curentUser.UniversalId = username[0].ToUpperInvariant();
            _curentUser.AuthDirName = directory.ToUpperInvariant();
            _curentUser.Email = principal.FindFirstValue(ClaimTypes.Email);
            _curentUser.UserName = principal.FindFirstValue(HmcrClaimTypes.UserName);
            _curentUser.FirstName = principal.FindFirstValue(ClaimTypes.GivenName);
            _curentUser.LastName = principal.FindFirstValue(ClaimTypes.Surname);
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
