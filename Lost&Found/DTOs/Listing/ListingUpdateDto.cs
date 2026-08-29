using System.ComponentModel.DataAnnotations;
using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Listing
{
    public class ListingUpdateDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public ListingType Type { get; set; }

        [Required]
        public Category Category { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Range(-90, 90)]
        public decimal? Latitude { get; set; }

        [Range(-180, 180)]
        public decimal? Longitude { get; set; }

        [MaxLength(500)]
        public string? Photo { get; set; }

        [MaxLength(500)]
        public string? LocationDescription { get; set; }
    }
}
