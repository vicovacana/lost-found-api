using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Conversation
{
    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ConversationStatus Status { get; set; }
        public int ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;
        public string? LocationDescription { get; set; }
    }
}
