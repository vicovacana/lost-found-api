using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Claim;
using Lost_Found.Models;
using Lost_Found.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _db;

        public ClaimService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ClaimDto> CreateAsync(int listingId, int userId)
        {
            var listingExists = await _db.Listings.AnyAsync(o => o.ListingId == listingId);
            if (!listingExists)
            {
                throw new NotFoundException($"Oglas {listingId} ne postoji.");
            }

            var alreadyExists = await _db.Claims.AnyAsync(p => p.ListingId == listingId && p.UserId == userId);
            if (alreadyExists)
            {
                throw new ConflictException("Već postoji potraživanje ovog korisnika za ovaj oglas.");
            }

            var claim = new Claim
            {
                ListingId = listingId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = ClaimStatus.Pending
            };

            _db.Claims.Add(claim);
            await _db.SaveChangesAsync();

            await _db.Entry(claim).Reference(p => p.User).LoadAsync();
            return ToDto(claim);
        }

        public async Task<IReadOnlyList<ClaimDto>> GetForListingAsync(int listingId, int currentUserId, bool isAdmin)
        {
            var listing = await _db.Listings.FirstOrDefaultAsync(o => o.ListingId == listingId)
                ?? throw new NotFoundException($"Oglas {listingId} ne postoji.");

            if (!isAdmin && listing.CreatorId != currentUserId)
            {
                throw new ForbiddenException("Samo vlasnik oglasa ili admin mogu da vide potraživanja.");
            }

            var claims = await _db.Claims
                .Include(p => p.User)
                .Where(p => p.ListingId == listingId)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            return claims.Select(ToDto).ToList();
        }

        public async Task<IReadOnlyList<ClaimDto>> GetMineAsync(int userId)
        {
            var claims = await _db.Claims
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return claims.Select(ToDto).ToList();
        }

        public async Task<ClaimDto> UpdateStatusAsync(int listingId, int userId, ClaimStatus newStatus)
        {
            var claim = await _db.Claims
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.ListingId == listingId && p.UserId == userId)
                ?? throw new NotFoundException("Potraživanje ne postoji.");

            claim.Status = newStatus;
            claim.ResolvedAt = DateTime.UtcNow;

            if (newStatus == ClaimStatus.Accepted)
            {
                var conversation = await _db.Conversations.FirstOrDefaultAsync(r => r.ListingId == listingId);
                if (conversation is not null && conversation.Status == ConversationStatus.Open)
                {
                    conversation.Status = ConversationStatus.Closed;
                }

                var remainingPending = await _db.Claims
                    .Where(p => p.ListingId == listingId && p.UserId != userId && p.Status == ClaimStatus.Pending)
                    .ToListAsync();

                foreach (var pending in remainingPending)
                {
                    pending.Status = ClaimStatus.Rejected;
                    pending.ResolvedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();

            return ToDto(claim);
        }

        public async Task WithdrawAsync(int listingId, int userId, int currentUserId)
        {
            var claim = await _db.Claims
                .FirstOrDefaultAsync(p => p.ListingId == listingId && p.UserId == userId)
                ?? throw new NotFoundException("Potraživanje ne postoji.");

            if (claim.UserId != currentUserId)
            {
                throw new ForbiddenException("Samo podnosilac može da povuče svoje potraživanje.");
            }

            if (claim.Status != ClaimStatus.Pending)
            {
                throw new ConflictException("Potraživanje koje je već rešeno ne može da se povuče.");
            }

            _db.Claims.Remove(claim);
            await _db.SaveChangesAsync();
        }

        private static ClaimDto ToDto(Claim claim) => new()
        {
            UserId = claim.UserId,
            Username = claim.User?.Username ?? string.Empty,
            ListingId = claim.ListingId,
            CreatedAt = claim.CreatedAt,
            Status = claim.Status,
            ResolvedAt = claim.ResolvedAt
        };
    }
}
