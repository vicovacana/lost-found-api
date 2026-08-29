using System.ComponentModel.DataAnnotations;

namespace Lost_Found.DTOs.Message
{
    public class MessageCreateDto
    {
        [Required, MaxLength(4000)]
        public string Content { get; set; } = string.Empty;
    }
}
