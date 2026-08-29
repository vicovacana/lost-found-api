using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Conversation;
using Lost_Found.Models;
using Lost_Found.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class ConversationService : IConversationService
    {
        private readonly ApplicationDbContext _db;

        public ConversationService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ConversationDto> OpenAsync(int listingId)
        {
            var listing = await _db.Listings.FirstOrDefaultAsync(o => o.ListingId == listingId)
                ?? throw new NotFoundException($"Oglas {listingId} ne postoji.");

            var alreadyExists = await _db.Conversations.AnyAsync(r => r.ListingId == listingId);
            if (alreadyExists)
            {
                throw new ConflictException("Razgovor za ovaj oglas već postoji.");
            }

            var conversation = new Conversation
            {
                ListingId = listingId,
                CreatedAt = DateTime.UtcNow,
                Status = ConversationStatus.Open
            };

            _db.Conversations.Add(conversation);
            await _db.SaveChangesAsync();

            return ToDto(conversation, listing, showLocationDescription: false);
        }

        public async Task<ConversationDto> GetForListingAsync(int listingId, int currentUserId, bool isAdmin)
        {
            var conversation = await _db.Conversations.Include(r => r.Listing).FirstOrDefaultAsync(r => r.ListingId == listingId)
                ?? throw new NotFoundException($"Razgovor za oglas {listingId} ne postoji.");

            await EnsureParticipantAsync(currentUserId, isAdmin, conversation);
            return await ToDtoAsync(conversation, conversation.Listing, currentUserId);
        }

        public async Task<ConversationDto> GetByIdAsync(int conversationId, int currentUserId, bool isAdmin)
        {
            var conversation = await _db.Conversations.Include(r => r.Listing).FirstOrDefaultAsync(r => r.ConversationId == conversationId)
                ?? throw new NotFoundException($"Razgovor {conversationId} ne postoji.");

            await EnsureParticipantAsync(currentUserId, isAdmin, conversation);
            return await ToDtoAsync(conversation, conversation.Listing, currentUserId);
        }

        public async Task<ConversationDto> UpdateStatusAsync(int conversationId, ConversationStatus newStatus)
        {
            var conversation = await _db.Conversations.Include(r => r.Listing).FirstOrDefaultAsync(r => r.ConversationId == conversationId)
                ?? throw new NotFoundException($"Razgovor {conversationId} ne postoji.");

            conversation.Status = newStatus;
            await _db.SaveChangesAsync();

            return ToDto(conversation, conversation.Listing, showLocationDescription: false);
        }

        public async Task<IReadOnlyList<ConversationDto>> GetMineAsync(int currentUserId, bool isAdmin)
        {
            IQueryable<Conversation> query = _db.Conversations.Include(r => r.Listing);

            if (!isAdmin)
            {
                query = query.Where(r => r.Listing.CreatorId == currentUserId
                    || _db.Claims.Any(p => p.ListingId == r.ListingId && p.UserId == currentUserId));
            }

            var conversations = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();

            var confirmedListingIdsForMe = await _db.Claims
                .Where(p => p.UserId == currentUserId && p.Status == ClaimStatus.Accepted
                    && conversations.Select(r => r.ListingId).Contains(p.ListingId))
                .Select(p => p.ListingId)
                .ToListAsync();
            var confirmedSet = confirmedListingIdsForMe.ToHashSet();

            return conversations.Select(r => ToDto(r, r.Listing, confirmedSet.Contains(r.ListingId))).ToList();
        }

        public async Task EnsureParticipantAsync(int conversationId, int currentUserId, bool isAdmin)
        {
            var conversation = await _db.Conversations.FirstOrDefaultAsync(r => r.ConversationId == conversationId)
                ?? throw new NotFoundException($"Razgovor {conversationId} ne postoji.");

            await EnsureParticipantAsync(currentUserId, isAdmin, conversation);
        }

        private async Task EnsureParticipantAsync(int currentUserId, bool isAdmin, Conversation conversation)
        {
            if (isAdmin)
            {
                return;
            }

            var isListingOwner = await _db.Listings.AnyAsync(o => o.ListingId == conversation.ListingId && o.CreatorId == currentUserId);
            if (isListingOwner)
            {
                return;
            }

            var hasClaim = await _db.Claims.AnyAsync(p => p.ListingId == conversation.ListingId && p.UserId == currentUserId);
            if (hasClaim)
            {
                return;
            }

            throw new ForbiddenException("Nemate pristup ovom razgovoru.");
        }

        private async Task<ConversationDto> ToDtoAsync(Conversation conversation, Listing listing, int currentUserId)
        {
            var isConfirmedClaimant = await _db.Claims.AnyAsync(p =>
                p.ListingId == conversation.ListingId && p.UserId == currentUserId && p.Status == ClaimStatus.Accepted);

            return ToDto(conversation, listing, isConfirmedClaimant);
        }

        private static ConversationDto ToDto(Conversation conversation, Listing listing, bool showLocationDescription) => new()
        {
            ConversationId = conversation.ConversationId,
            CreatedAt = conversation.CreatedAt,
            Status = conversation.Status,
            ListingId = conversation.ListingId,
            ListingTitle = listing.Title,
            LocationDescription = showLocationDescription ? listing.LocationDescription : null
        };
    }
}
