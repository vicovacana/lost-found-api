using Lost_Found.DTOs.User;

namespace Lost_Found.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int userId);
        Task<UserDto> UpdateAsync(int userId, UserUpdateDto dto);
    }
}
