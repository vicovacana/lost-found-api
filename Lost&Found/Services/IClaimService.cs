using Lost_Found.DTOs.Claim;
using Lost_Found.Models.Enums;

namespace Lost_Found.Services
{
    public interface IClaimService
    {
        Task<ClaimDto> CreateAsync(int listingId, int userId);
        Task<IReadOnlyList<ClaimDto>> GetForListingAsync(int listingId, int currentUserId, bool isAdmin);
        Task<IReadOnlyList<ClaimDto>> GetMineAsync(int userId);
        Task<ClaimDto> UpdateStatusAsync(int listingId, int userId, ClaimStatus newStatus);
        Task WithdrawAsync(int listingId, int userId, int currentUserId);
    }
}
