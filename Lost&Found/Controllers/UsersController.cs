using Lost_Found.DTOs.User;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    [ApiController]
    [Route("api/korisnici")]
    [Authorize]
    public class UsersController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            return Ok(await _userService.GetByIdAsync(id));
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            return Ok(await _userService.GetByIdAsync(CurrentUserId));
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserDto>> UpdateMe(UserUpdateDto dto)
        {
            return Ok(await _userService.UpdateAsync(CurrentUserId, dto));
        }
    }
}
