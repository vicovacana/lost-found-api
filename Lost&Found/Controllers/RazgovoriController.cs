using Lost_Found.DTOs.Razgovor;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    [ApiController]
    [Authorize]
    public class RazgovoriController : ApiControllerBase
    {
        private readonly IRazgovorService _razgovorService;

        public RazgovoriController(IRazgovorService razgovorService)
        {
            _razgovorService = razgovorService;
        }

        [HttpGet("api/razgovori/mine")]
        public async Task<ActionResult<IReadOnlyList<RazgovorDto>>> GetMine()
        {
            return Ok(await _razgovorService.GetMineAsync(CurrentKorisnikId, IsAdmin));
        }

        [HttpPost("api/oglasi/{oglasId:int}/razgovor")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RazgovorDto>> Open(int oglasId)
        {
            var result = await _razgovorService.OpenAsync(oglasId);
            return CreatedAtAction(nameof(GetById), new { id = result.RazgovorId }, result);
        }

        [HttpGet("api/oglasi/{oglasId:int}/razgovor")]
        public async Task<ActionResult<RazgovorDto>> GetForOglas(int oglasId)
        {
            return Ok(await _razgovorService.GetForOglasAsync(oglasId, CurrentKorisnikId, IsAdmin));
        }

        [HttpGet("api/razgovori/{id:int}")]
        public async Task<ActionResult<RazgovorDto>> GetById(int id)
        {
            return Ok(await _razgovorService.GetByIdAsync(id, CurrentKorisnikId, IsAdmin));
        }

        [HttpPatch("api/razgovori/{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RazgovorDto>> UpdateStatus(int id, AzurirajStatusRazgovoraDto dto)
        {
            return Ok(await _razgovorService.UpdateStatusAsync(id, dto.StatusRazgovora));
        }
    }
}
