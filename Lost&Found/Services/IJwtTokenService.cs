using Lost_Found.Models;

namespace Lost_Found.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
