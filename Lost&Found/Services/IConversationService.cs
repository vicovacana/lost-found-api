using Lost_Found.DTOs.Conversation;
using Lost_Found.Models.Enums;

namespace Lost_Found.Services
{
    public interface IConversationService
    {
        Task<ConversationDto> OpenAsync(int listingId);
        Task<ConversationDto> GetForListingAsync(int listingId, int currentUserId, bool isAdmin);
        Task<ConversationDto> GetByIdAsync(int conversationId, int currentUserId, bool isAdmin);
        Task<ConversationDto> UpdateStatusAsync(int conversationId, ConversationStatus newStatus);
        Task<IReadOnlyList<ConversationDto>> GetMineAsync(int currentUserId, bool isAdmin);
        Task EnsureParticipantAsync(int conversationId, int currentUserId, bool isAdmin);
    }
}
