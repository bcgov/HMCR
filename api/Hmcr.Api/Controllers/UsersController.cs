using Hmcr.Domain.Services;
using Hmcr.Model;
using Hmcr.Model.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

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
            return Ok(new UserStatusDto());
        }
    }
}
