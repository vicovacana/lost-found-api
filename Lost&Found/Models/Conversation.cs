using Lost_Found.Models.Enums;

namespace Lost_Found.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public ConversationStatus Status { get; set; }

        public int ListingId { get; set; }
        public Listing Listing { get; set; } = null!;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
