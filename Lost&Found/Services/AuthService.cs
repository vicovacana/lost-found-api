using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Auth;
using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext db, IJwtTokenService jwtTokenService, IConfiguration configuration)
        {
            _db = db;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var exists = await _db.Users.AnyAsync(k =>
                k.Username == dto.Username || k.Email == dto.Email);
            if (exists)
            {
                throw new ConflictException("Korisničko ime ili email su već zauzeti.");
            }

            var user = new StandardUser
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.StandardUsers.Add(user);
            await _db.SaveChangesAsync();

            return BuildResponse(user);
        }

        public async Task<AuthResponseDto> RegisterAdminAsync(RegisterAdminDto dto)
        {
            var secretCode = _configuration["Admin:RegistrationSecret"];
            if (string.IsNullOrEmpty(secretCode) || dto.SecretCode != secretCode)
            {
                throw new ValidationException("Pogrešan tajni kod.");
            }

            var exists = await _db.Users.AnyAsync(k =>
                k.Username == dto.Username || k.Email == dto.Email);
            if (exists)
            {
                throw new ConflictException("Korisničko ime ili email su već zauzeti.");
            }

            var admin = new Admin
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            _db.Admins.Add(admin);
            await _db.SaveChangesAsync();

            return BuildResponse(admin);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(k => k.Username == dto.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                throw new ValidationException("Pogrešno korisničko ime ili lozinka.");
            }

            return BuildResponse(user);
        }

        private AuthResponseDto BuildResponse(User user) => new()
        {
            Token = _jwtTokenService.GenerateToken(user),
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user is Admin ? "Admin" : "StandardUser"
        };
    }
}
