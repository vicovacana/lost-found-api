using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Claim
{
    public class ClaimDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int ListingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ClaimStatus Status { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
