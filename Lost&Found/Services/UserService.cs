using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.User;
using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _db;

        public UserService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<UserDto>> GetAllAsync()
        {
            var users = await _db.Users.OrderBy(k => k.UserId).ToListAsync();
            return users.Select(ToDto).ToList();
        }

        public async Task<UserDto> GetByIdAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId)
                ?? throw new NotFoundException($"Korisnik {userId} ne postoji.");

            return ToDto(user);
        }

        public async Task<UserDto> UpdateAsync(int userId, UserUpdateDto dto)
        {
            var user = await _db.Users.FindAsync(userId)
                ?? throw new NotFoundException($"Korisnik {userId} ne postoji.");

            var taken = await _db.Users.AnyAsync(k =>
                k.UserId != userId &&
                (k.Username == dto.Username || k.Email == dto.Email));
            if (taken)
            {
                throw new ConflictException("Korisničko ime ili email su već zauzeti.");
            }

            user.Username = dto.Username;
            user.Email = dto.Email;
            await _db.SaveChangesAsync();

            return ToDto(user);
        }

        public async Task<UserDto> CreateAdminAsync(CreateAdminDto dto)
        {
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

            return ToDto(admin);
        }

        public async Task DeleteAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId)
                ?? throw new NotFoundException($"Korisnik {userId} ne postoji.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        private static UserDto ToDto(User user) => new()
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Role = user is Admin ? "Admin" : "StandardUser"
        };
    }
}
