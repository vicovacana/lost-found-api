namespace Lost_Found.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
