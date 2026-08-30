using Lost_Found.Common;
using Lost_Found.Data;
using Lost_Found.DTOs.Listing;
using Lost_Found.Models;
using Lost_Found.Models.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Services
{
    public class ListingService : IListingService
    {
        private static readonly Dictionary<string, string> AllowedContentTypes = new()
        {
            ["image/jpeg"] = ".jpg",
            ["image/png"] = ".png",
            ["image/webp"] = ".webp",
            ["image/gif"] = ".gif",
        };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024;

        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public ListingService(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IReadOnlyList<ListingDto>> GetAllAsync(ListingType? type, int? creatorId, int? adminId, Category? category, string? city, bool? activeOnly)
        {
            var query = _db.Listings.Include(o => o.Creator).Include(o => o.Admin).Where(o => !o.IsDeleted);

            if (type.HasValue) query = query.Where(o => o.Type == type.Value);
            if (creatorId.HasValue) query = query.Where(o => o.CreatorId == creatorId.Value);
            if (adminId.HasValue) query = query.Where(o => o.AdminId == adminId.Value);
            if (category.HasValue) query = query.Where(o => o.Category == category.Value);
            if (!string.IsNullOrWhiteSpace(city))
            {
                var cityTrimmed = city.Trim();
                query = query.Where(o => EF.Functions.ILike(o.City, cityTrimmed));
            }
            if (activeOnly.HasValue)
            {
                query = activeOnly.Value
                    ? query.Where(o => o.Conversation == null || o.Conversation.Status != ConversationStatus.Closed)
                    : query.Where(o => o.Conversation != null && o.Conversation.Status == ConversationStatus.Closed);
            }

            var listings = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
            return listings.Select(o => ToDto(o)).ToList();
        }

        public async Task<ListingDto> GetByIdAsync(int listingId, int? currentUserId, bool isAdmin)
        {
            var listing = await _db.Listings.Include(o => o.Creator).Include(o => o.Admin)
                .FirstOrDefaultAsync(o => o.ListingId == listingId && !o.IsDeleted)
                ?? throw new NotFoundException($"Oglas {listingId} ne postoji.");

            var canSeeLocationDescription = isAdmin || (currentUserId.HasValue && listing.CreatorId == currentUserId.Value);
            return ToDto(listing, canSeeLocationDescription);
        }

        public async Task<ListingDto> CreateAsync(int creatorId, ListingCreateDto dto)
        {
            var listing = new Listing
            {
                Title = dto.Title,
                Description = dto.Description,
                Type = dto.Type,
                Category = dto.Category,
                City = dto.City,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Photo = dto.Photo,
                LocationDescription = NormalizeLocationDescription(dto.Type, dto.LocationDescription),
                CreatorId = creatorId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Listings.Add(listing);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(listing.ListingId, creatorId, isAdmin: false);
        }

        public async Task<ListingDto> UpdateAsync(int listingId, int currentUserId, bool isAdmin, ListingUpdateDto dto)
        {
            var listing = await _db.Listings.Include(o => o.Creator).Include(o => o.Admin)
                .FirstOrDefaultAsync(o => o.ListingId == listingId && !o.IsDeleted)
                ?? throw new NotFoundException($"Oglas {listingId} ne postoji.");

            if (!isAdmin && listing.CreatorId != currentUserId)
            {
                throw new ForbiddenException("Samo vlasnik oglasa ili admin mogu da ga izmene.");
            }

            listing.Title = dto.Title;
            listing.Description = dto.Description;
            listing.Type = dto.Type;
            listing.Category = dto.Category;
            listing.City = dto.City;
            listing.Latitude = dto.Latitude;
            listing.Longitude = dto.Longitude;
            listing.Photo = dto.Photo;
            listing.LocationDescription = NormalizeLocationDescription(dto.Type, dto.LocationDescription);

            await _db.SaveChangesAsync();

            return ToDto(listing, includeLocationDescription: true);
        }

        public async Task DeleteAsync(int listingId, int currentUserId, bool isAdmin)
        {
            var listing = await _db.Listings.FirstOrDefaultAsync(o => o.ListingId == listingId && !o.IsDeleted)
                ?? throw new NotFoundException($"Oglas {listingId} ne postoji.");

            if (!isAdmin && listing.CreatorId != currentUserId)
            {
                throw new ForbiddenException("Samo vlasnik oglasa ili admin mogu da ga obrišu.");
            }

            listing.IsDeleted = true;
            await _db.SaveChangesAsync();
        }

        public async Task<string> SavePhotoAsync(IFormFile file, string baseUrl)
        {
            if (file is null || file.Length == 0)
            {
                throw new ValidationException("Fajl nije poslat.");
            }

            if (file.Length > MaxFileSizeBytes)
            {
                throw new ValidationException("Fotografija ne sme biti veća od 5 MB.");
            }

            if (!AllowedContentTypes.TryGetValue(file.ContentType, out var extension))
            {
                throw new ValidationException("Dozvoljeni formati su JPG, PNG, WEBP i GIF.");
            }

            var directory = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "uploads", "oglasi");
            Directory.CreateDirectory(directory);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var path = Path.Combine(directory, fileName);

            await using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"{baseUrl}/uploads/oglasi/{fileName}";
        }

        private static string? NormalizeLocationDescription(ListingType type, string? locationDescription)
        {
            if (type != ListingType.Found || string.IsNullOrWhiteSpace(locationDescription))
            {
                return null;
            }

            return locationDescription.Trim();
        }

        private static ListingDto ToDto(Listing listing, bool includeLocationDescription = false) => new()
        {
            ListingId = listing.ListingId,
            Title = listing.Title,
            Description = listing.Description,
            CreatedAt = listing.CreatedAt,
            Type = listing.Type,
            Category = listing.Category,
            City = listing.City,
            Latitude = listing.Latitude,
            Longitude = listing.Longitude,
            Photo = listing.Photo,
            LocationDescription = includeLocationDescription ? listing.LocationDescription : null,
            CreatorId = listing.CreatorId,
            CreatorUsername = listing.Creator?.Username ?? string.Empty,
            AdminId = listing.AdminId,
            AdminUsername = listing.Admin?.Username
        };
    }
}
