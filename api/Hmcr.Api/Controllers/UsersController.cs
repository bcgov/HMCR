using Hmcr.Api.Authorization;
using Hmcr.Api.Controllers.Base;
using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.Keycloak;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/users")]
    [ApiController]
    public class UsersController : HmcrControllerBase
    {
        private IUserService _userService;
        private IKeycloakService _keyCloakService;
        private HmcrCurrentUser _currentUser;

        public UsersController(IUserService userService, IKeycloakService keyCloakService, HmcrCurrentUser currentUser)
        {
            _userService = userService;
            _keyCloakService = keyCloakService;
            _currentUser = currentUser;
        }

        [HttpGet("current")]
        public ActionResult<UserCurrentDto> GetCurrentUser()
        {
            return Ok(_currentUser.UserInfo);
        }

        [HttpGet("usertypes")]
        public ActionResult<IEnumerable<UserTypeDto>> GetUserTypes()
        {
            var userTypes = new List<UserTypeDto>()
            {
                new UserTypeDto
                {
                    UserTypeId = UserTypeDto.INTERNAL,
                    UserType = UserTypeDto.IDIR
                },
                new UserTypeDto
                {
                    UserTypeId = UserTypeDto.BUSINESS,
                    UserType = UserTypeDto.BCeId
                }
            };

            return Ok(userTypes);
        }

        [HttpGet("userstatus")]
        public ActionResult<IEnumerable<UserTypeDto>> GetUserStatus()
        {
            var statuses = new List<UserStatusDto>()
            {
                new UserStatusDto
                {
                    UserStatusId = UserStatusDto.ACTIVE,
                    UserStatus = UserStatusDto.ACTIVE
                },
                new UserStatusDto
                {
                    UserStatusId = UserStatusDto.INACTIVE,
                    UserStatus = UserStatusDto.INACTIVE
                }
            };

            return Ok(statuses);
        }

        /// <summary>
        /// Search users
        /// </summary>
        /// <param name="serviceAreas">Comma separated service area numbers. Example: serviceareas=1,2</param>
        /// <param name="userTypes">Comma separated user types. Example: usertypes=INTERNAL,BUSINESS</param>
        /// <param name="searchText">Search text for first name, last name, orgnization name, username</param>
        /// <param name="isActive">True or false</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="orderBy">Order by column(s). Example: orderby=username</param>
        /// <param name="direction">Oder by direction.  Example: asc, desc</param>
        /// <returns></returns>
        [HttpGet]
        [RequiresPermission(Permissions.UserRead)]
        public async Task<ActionResult<PagedDto<UserSearchDto>>> GetUsersAsync(
            [FromQuery]string? serviceAreas, [FromQuery]string? userTypes, [FromQuery]string searchText, [FromQuery]bool? isActive,
            [FromQuery]int pageSize, [FromQuery]int pageNumber, [FromQuery]string orderBy = "username", [FromQuery]string direction = "")
        {
            return Ok(await _userService.GetUsersAsync(serviceAreas.ToDecimalArray(), userTypes.ToStringArray(), searchText, isActive, pageSize, pageNumber, orderBy, direction));
        }

        [HttpGet("{id}", Name = "GetUser")]
        [RequiresPermission(Permissions.UserRead)]
        public async Task<ActionResult<UserDto>> GetUsersAsync(decimal id)
        {
            var user = await _userService.GetUserAsync(id);

            if (user == null)
                return NotFound();

            return user;
        }

        [HttpGet("bceidaccount/{userType}/{username}", Name = "GetBceidAccount")]
        [RequiresPermission(Permissions.UserWrite)]
        public async Task<ActionResult<UserBceidAccountDto>> GetBceidAccountAsync(string username, string userType)
        {
            var bceidAccount = await _userService.GetBceidAccountAsync(username, userType);

            if (bceidAccount == null)
                return NotFound();

            return bceidAccount;
        }

        [HttpPost]
        [RequiresPermission(Permissions.UserWrite)]
        public async Task<ActionResult<UserDto>> CreateUser(UserCreateDto user)
        {
            var response = await _userService.CreateUserAsync(user);

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return CreatedAtRoute("GetUser", new { id = response.SystemUserId }, await _userService.GetUserAsync(response.SystemUserId));
        }

        [HttpPut("{id}")]
        [RequiresPermission(Permissions.UserWrite)]
        public async Task<ActionResult> UpdateUser(decimal id, UserUpdateDto user)
        {
            if (id != user.SystemUserId)
            {
                throw new Exception($"The system user ID from the query string does not match that of the body.");
            }

            var response = await _userService.UpdateUserAsync(user);

            if (response.NotFound)
            {
                return NotFound();
            }

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return NoContent();
        }


        [HttpDelete("{id}")]
        [RequiresPermission(Permissions.UserWrite)]
        public async Task<ActionResult> DeleteUser(decimal id, UserDeleteDto user)
        {
            if (id != user.SystemUserId)
            {
                throw new Exception($"The system user ID from the query string does not match that of the body.");
            }

            var response = await _userService.DeleteUserAsync(user);

            if (response.NotFound)
            {
                return NotFound();
            }

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return NoContent();
        }

        #region API Client
        [HttpGet("api-client", Name = "GetUserKeycloakClient")]
        public async Task<ActionResult<KeycloakClientDto>> GetUserKeycloakClient()
        {
            var client = await _keyCloakService.GetUserClientAsync();

            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        [HttpPost("api-client")]
        public async Task<ActionResult<KeycloakClientDto>> CreateUserKeycloakClient()
        {
            var response = await _keyCloakService.CreateUserClientAsync();

            if (response.Errors.Count > 0)
            {
                return ValidationUtils.GetValidationErrorResult(response.Errors, ControllerContext);
            }

            return CreatedAtRoute("GetUserKeycloakClient", await _keyCloakService.GetUserClientAsync());
        }

        [HttpPost("api-client/secret")]
        public async Task<ActionResult> RegenerateUserKeycloakClientSecret()
        {
            var response = await _keyCloakService.RegenerateUserClientSecretAsync();

            if (response.NotFound)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(response.Error))
            {
                return ValidationUtils.GetValidationErrorResult(ControllerContext,
                     StatusCodes.Status500InternalServerError, "Unable to regenerate Keycloak client secret", response.Error);
            }

            return NoContent();
        }
        #endregion
    }
}
