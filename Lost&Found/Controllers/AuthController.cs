using Lost_Found.DTOs.Auth;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            return Ok(await _authService.RegisterAsync(dto));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            return Ok(await _authService.LoginAsync(dto));
        }

        [HttpPost("register-admin")]
        public async Task<ActionResult<AuthResponseDto>> RegisterAdmin(RegisterAdminDto dto)
        {
            return Ok(await _authService.RegisterAdminAsync(dto));
        }
    }
}
