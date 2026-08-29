using System.ComponentModel.DataAnnotations;
using Lost_Found.Models.Enums;

namespace Lost_Found.DTOs.Conversation
{
    public class UpdateConversationStatusDto
    {
        [Required]
        public ConversationStatus Status { get; set; }
    }
}
