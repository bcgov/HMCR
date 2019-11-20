using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos;
using Hmcr.Model.Dtos.User;
using Hmcr.Model.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private HmcrCurrentUser _currentUser;

        public UsersController(IUserService userService, HmcrCurrentUser currentUser)
        {
            _userService = userService;
            _currentUser = currentUser;
        }

        [HttpGet("current")]
        public ActionResult<UserCurrentDto> GetCurrentUser()
        {
            return Ok(_currentUser.UserInfo);
        }

        [HttpGet("usertypes")]
        public ActionResult<UserTypeDto> GetUserTypes()
        {
            return Ok(new UserTypeDto());
        }

        [HttpGet("userstatus")]
        public ActionResult<UserTypeDto> GetUserStatus()
        {
            return Ok(new UserStatusDto().UserStatuses);
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
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedDto<UserSearchDto>>> GetUsersAsync(
            [FromQuery]string? serviceAreas, [FromQuery]string? userTypes, [FromQuery]string searchText, [FromQuery]bool? isActive,
            [FromQuery]int pageSize, [FromQuery]int pageNumber, [FromQuery]string orderBy)
        {
            orderBy ??= "Username";

            return Ok(await _userService.GetUsersAsync(serviceAreas.ToDecimalArray(), userTypes.ToStringArray(), searchText, isActive, pageSize, pageNumber, orderBy));
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<UserDto>> GetUsersAsync(decimal id)
        {
            return await _userService.GetUserAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserCreateDto user)
        {
            var systemUserId = await _userService.CreateUserAsync(user);

            return CreatedAtRoute("GetUser", new { id = systemUserId }, await _userService.GetUserAsync(systemUserId));
        }
    }
}
