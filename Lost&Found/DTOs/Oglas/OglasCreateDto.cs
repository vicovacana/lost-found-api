using System.ComponentModel.DataAnnotations;
using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Oglas
{
    public class OglasCreateDto
    {
        [Required, MaxLength(150)]
        public string Naziv { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Opis { get; set; } = string.Empty;

        [Required]
        public TipOglasa Tip { get; set; }

        [Required]
        public Kategorija Kategorija { get; set; }

        [Required, MaxLength(100)]
        public string Grad { get; set; } = string.Empty;

        [Range(-90, 90)]
        public decimal? Latitude { get; set; }

        [Range(-180, 180)]
        public decimal? Longitude { get; set; }

        [MaxLength(500)]
        public string? Fotografija { get; set; }

        [MaxLength(1000)]
        public string? OpisLokacije { get; set; }
    }
}
