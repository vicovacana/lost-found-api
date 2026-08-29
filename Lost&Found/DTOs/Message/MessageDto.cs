namespace Lost_Found.DTOs.Message
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int ConversationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
