using Hmcr.Api.Extensions;
using Hmcr.Bceid;
using Hmcr.Data.Database.Entities;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private ILogger<HmcrJwtBearerEvents> _logger;

        public HmcrJwtBearerEvents(IWebHostEnvironment env, IUserService userService,
            HmcrCurrentUser currentUser, IBceidApi bceid, ILogger<HmcrJwtBearerEvents> logger) : base()
        {
            _userService = userService;
            _curentUser = currentUser;
            _bceid = bceid;
            _logger = logger;
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
            if (!(await PopulateCurrentUserFromDb(context.Principal)))
            {
                context.Fail("Access Denied");
                return;
            }

            _curentUser.UserInfo = await _userService.GetCurrentUserAsync();

            AddClaimsFromUserInfo(context.Principal, _curentUser.UserInfo);
        }

        private async Task<bool> PopulateCurrentUserFromDb(ClaimsPrincipal principal)
        {
            var isApiClient = false;
            bool.TryParse(principal.FindFirstValue(HmcrClaimTypes.KcIsApiClient), out isApiClient);
            isApiClient = true;

            //preferred_username token has a form of "{guid}@{directory}".
            //var preferredUsername = isApiClient ? principal.FindFirstValue(HmcrClaimTypes.KcApiUsername) : principal.FindFirstValue(HmcrClaimTypes.KcUsername);
            var preferredUsername = principal.FindFirstValue(HmcrClaimTypes.KcUsername);
            var username = principal.FindFirstValue(HmcrClaimTypes.KcApiUsername);
            var directory = preferredUsername.Split("@")[1].ToUpperInvariant();

            //var userGuidClaim = directory.ToUpperInvariant() == UserTypeDto.IDIR ? HmcrClaimTypes.KcIdirGuid : HmcrClaimTypes.KcBceidGuid;
            //var userGuid = new Guid(principal.FindFirstValue(userGuidClaim));

            username = principal.FindFirstValue(HmcrClaimTypes.KcUsername).Split("@")[0].ToUpperInvariant(); ;
            var userGuid = new Guid(Guid.Parse(username).ToString());
            var email = principal.FindFirstValue(ClaimTypes.Email)?.ToUpperInvariant();

            var user = await _userService.GetActiveUserEntityAsync(userGuid);
            if (user == null)
            {
                _logger.LogWarning($"Access Denied - User[{username}/{userGuid}] does not exist");
                return false;
            }

            //When it's an Api client, we don't want to use the username from the token because it's a hard-code value.
            //This is to support the scenario where username has changed for the GUID. Note GUID never changes and unique but username can change.
            if (isApiClient)
            {
                username = user.Username;
                email = user.Email;
            }

            if (directory == "IDIR")
            {
                _curentUser.UserGuid = userGuid;
                _curentUser.UserType = UserTypeDto.INTERNAL;
            }
            else
            {
                _curentUser.UserGuid = userGuid;
                _curentUser.BusinessGuid = user.BusinessGuid;
                _curentUser.BusinessLegalName = user.Party.BusinessLegalName;
                _curentUser.BusinessNumber = user.Party.BusinessNumber ?? 0;
                _curentUser.UserType = UserTypeDto.BUSINESS;
            }

            _curentUser.Username = username;
            _curentUser.AuthDirName = directory;
            _curentUser.Email = email;
            _curentUser.FirstName = user.FirstName;
            _curentUser.LastName = user.LastName;
            _curentUser.ApiClientId = user.ApiClientId;

            if (isApiClient) //no db update, so everything's done.
            {
                _logger.LogInformation($"ApiClient Login - {preferredUsername}/{username}");
                return true;
            }
            
            if (user.Username.ToUpperInvariant() != username || user.Email.ToUpperInvariant() != email) //when the info changed, update db with the latest info from bceid web service
            {
                _logger.LogWarning($"Username/Email changed from {user.Username}/{user.Email} to {username}/{email}.");
                await _userService.UpdateUserFromBceidAsync(userGuid, username, user.UserType, user.ConcurrencyControlNumber);
            }

            return true;
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

            claims.Add(new Claim(ClaimTypes.Name, _curentUser.Username));

            principal.AddIdentity(new ClaimsIdentity(claims));
        }
    }
}
