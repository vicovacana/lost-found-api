using Lost_Found.DTOs.Message;

namespace Lost_Found.Services
{
    public interface IMessageService
    {
        Task<IReadOnlyList<MessageDto>> GetForConversationAsync(int conversationId, int currentUserId, bool isAdmin);
        Task<MessageDto> CreateAsync(int conversationId, int userId, bool isAdmin, MessageCreateDto dto);
    }
}
