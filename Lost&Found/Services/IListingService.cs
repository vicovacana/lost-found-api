using Lost_Found.DTOs.Listing;
using Lost_Found.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Lost_Found.Services
{
    public interface IListingService
    {
        Task<IReadOnlyList<ListingDto>> GetAllAsync(ListingType? type, int? creatorId, int? adminId, Category? category, string? city, bool? activeOnly);
        Task<ListingDto> GetByIdAsync(int listingId, int? currentUserId, bool isAdmin);
        Task<ListingDto> CreateAsync(int creatorId, ListingCreateDto dto);
        Task<ListingDto> UpdateAsync(int listingId, int currentUserId, bool isAdmin, ListingUpdateDto dto);
        Task DeleteAsync(int listingId, int currentUserId, bool isAdmin);
        Task<ListingDto> AssignAdminAsync(int listingId, int? adminId);
        Task<string> SavePhotoAsync(IFormFile file, string baseUrl);
    }
}
