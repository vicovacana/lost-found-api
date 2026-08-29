using Lost_Found.DTOs.User;

namespace Lost_Found.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserDto>> GetAllAsync();
        Task<UserDto> GetByIdAsync(int userId);
        Task<UserDto> UpdateAsync(int userId, UserUpdateDto dto);
        Task<UserDto> CreateAdminAsync(CreateAdminDto dto);
        Task DeleteAsync(int userId);
    }
}
