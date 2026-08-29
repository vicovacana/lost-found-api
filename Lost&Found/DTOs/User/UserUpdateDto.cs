using System.ComponentModel.DataAnnotations;

namespace Lost_Found.DTOs.User
{
    public class UserUpdateDto
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;
    }
}
