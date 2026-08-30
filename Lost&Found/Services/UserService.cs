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
