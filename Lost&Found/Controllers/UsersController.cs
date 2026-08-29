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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
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

        [HttpPost("admins")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateAdmin(CreateAdminDto dto)
        {
            var result = await _userService.CreateAdminAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
