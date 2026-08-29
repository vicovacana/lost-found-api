using Lost_Found.Models.Enums;

namespace Lost_Found.Models
{
    public class Claim
    {
        public int UserId { get; set; }
        public StandardUser User { get; set; } = null!;

        public int ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public ClaimStatus Status { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
