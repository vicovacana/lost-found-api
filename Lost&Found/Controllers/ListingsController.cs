using Lost_Found.DTOs.Listing;
using Lost_Found.Models.Enums;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    [ApiController]
    [Route("api/oglasi")]
    public class ListingsController : ApiControllerBase
    {
        private readonly IListingService _listingService;

        public ListingsController(IListingService listingService)
        {
            _listingService = listingService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<ListingDto>>> GetAll(
            [FromQuery] ListingType? tip, [FromQuery] int? kreatorId, [FromQuery] int? adminId,
            [FromQuery] Category? kategorija, [FromQuery] string? grad, [FromQuery] bool? samoAktivni)
        {
            return Ok(await _listingService.GetAllAsync(tip, kreatorId, adminId, kategorija, grad, samoAktivni));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ListingDto>> GetById(int id)
        {
            return Ok(await _listingService.GetByIdAsync(id, CurrentUserIdOrNull, IsAdmin));
        }

        [HttpPost]
        [Authorize(Roles = "StandardUser")]
        public async Task<ActionResult<ListingDto>> Create(ListingCreateDto dto)
        {
            var result = await _listingService.CreateAsync(CurrentUserId, dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ListingId }, result);
        }

        [HttpPost("fotografije")]
        [Authorize(Roles = "StandardUser")]
        [RequestSizeLimit(5_000_000)]
        public async Task<ActionResult<UploadPhotoResponseDto>> UploadPhoto(IFormFile fajl)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var url = await _listingService.SavePhotoAsync(fajl, baseUrl);
            return Ok(new UploadPhotoResponseDto { Url = url });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ListingDto>> Update(int id, ListingUpdateDto dto)
        {
            return Ok(await _listingService.UpdateAsync(id, CurrentUserId, IsAdmin, dto));
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _listingService.DeleteAsync(id, CurrentUserId, IsAdmin);
            return NoContent();
        }
    }
}
