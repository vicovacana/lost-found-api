using Lost_Found.DTOs.Claim;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    [ApiController]
    [Authorize]
    public class ClaimsController : ApiControllerBase
    {
        private readonly IClaimService _claimService;

        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        [HttpPost("api/oglasi/{oglasId:int}/potrazivanja")]
        [Authorize(Roles = "StandardUser")]
        public async Task<ActionResult<ClaimDto>> Create(int oglasId)
        {
            var result = await _claimService.CreateAsync(oglasId, CurrentUserId);
            return CreatedAtAction(nameof(GetForListing), new { oglasId }, result);
        }

        [HttpGet("api/oglasi/{oglasId:int}/potrazivanja")]
        public async Task<ActionResult<IReadOnlyList<ClaimDto>>> GetForListing(int oglasId)
        {
            return Ok(await _claimService.GetForListingAsync(oglasId, CurrentUserId, IsAdmin));
        }

        [HttpGet("api/potrazivanja/mine")]
        public async Task<ActionResult<IReadOnlyList<ClaimDto>>> GetMine()
        {
            return Ok(await _claimService.GetMineAsync(CurrentUserId));
        }

        [HttpPatch("api/oglasi/{oglasId:int}/potrazivanja/{korisnikId:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClaimDto>> UpdateStatus(int oglasId, int korisnikId, UpdateClaimStatusDto dto)
        {
            return Ok(await _claimService.UpdateStatusAsync(oglasId, korisnikId, dto.Status));
        }

        [HttpDelete("api/oglasi/{oglasId:int}/potrazivanja/{korisnikId:int}")]
        public async Task<IActionResult> Withdraw(int oglasId, int korisnikId)
        {
            await _claimService.WithdrawAsync(oglasId, korisnikId, CurrentUserId);
            return NoContent();
        }
    }
}
