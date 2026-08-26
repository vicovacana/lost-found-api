using Lost_Found.DTOs.Auth;

namespace Lost_Found.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> RegisterAdminAsync(RegisterAdminDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
