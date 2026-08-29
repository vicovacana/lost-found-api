using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Message;
using Lost_Found.Models;
using Lost_Found.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConversationService _conversationService;

        public MessageService(ApplicationDbContext db, IConversationService conversationService)
        {
            _db = db;
            _conversationService = conversationService;
        }

        public async Task<IReadOnlyList<MessageDto>> GetForConversationAsync(int conversationId, int currentUserId, bool isAdmin)
        {
            await _conversationService.EnsureParticipantAsync(conversationId, currentUserId, isAdmin);

            var messages = await _db.Messages
                .Include(p => p.User)
                .Where(p => p.ConversationId == conversationId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            return messages.Select(ToDto).ToList();
        }

        public async Task<MessageDto> CreateAsync(int conversationId, int userId, bool isAdmin, MessageCreateDto dto)
        {
            await _conversationService.EnsureParticipantAsync(conversationId, userId, isAdmin);

            var conversation = await _db.Conversations.FirstOrDefaultAsync(r => r.ConversationId == conversationId)
                ?? throw new NotFoundException($"Razgovor {conversationId} ne postoji.");

            if (conversation.Status == ConversationStatus.Closed)
            {
                throw new ConflictException("Razgovor je zatvoren, nije moguće slati poruke.");
            }

            var message = new Message
            {
                ConversationId = conversationId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _db.Messages.Add(message);
            await _db.SaveChangesAsync();

            await _db.Entry(message).Reference(p => p.User).LoadAsync();
            return ToDto(message);
        }

        private static MessageDto ToDto(Message message) => new()
        {
            MessageId = message.MessageId,
            UserId = message.UserId,
            Username = message.User?.Username ?? string.Empty,
            ConversationId = message.ConversationId,
            CreatedAt = message.CreatedAt,
            Content = message.Content
        };
    }
}
