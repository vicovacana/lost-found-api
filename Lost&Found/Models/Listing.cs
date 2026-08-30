using Lost_Found.Models.Enums;

namespace Lost_Found.Models
{
    public class Listing
    {
        public int ListingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ListingType Type { get; set; }
        public Category Category { get; set; }
        public string City { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? Photo { get; set; }
        public string? LocationDescription { get; set; }
        public bool IsDeleted { get; set; }

        public int CreatorId { get; set; }
        public StandardUser Creator { get; set; } = null!;

        public int? AdminId { get; set; }
        public Admin? Admin { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
        public Conversation? Conversation { get; set; }
    }
}
