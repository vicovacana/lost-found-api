using System.ComponentModel.DataAnnotations;

namespace Lost_Found.DTOs.Auth
{
    public class RegisterAdminDto
    {
        [Required, MaxLength(50)]
        public string KorisnickoIme { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Lozinka { get; set; } = string.Empty;

        [Required]
        public string TajniKod { get; set; } = string.Empty;
    }
}
