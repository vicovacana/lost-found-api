using Lost_Found.DTOs.Conversation;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    [ApiController]
    [Authorize]
    public class ConversationsController : ApiControllerBase
    {
        private readonly IConversationService _conversationService;

        public ConversationsController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet("api/razgovori/mine")]
        public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetMine()
        {
            return Ok(await _conversationService.GetMineAsync(CurrentUserId, IsAdmin));
        }

        [HttpPost("api/oglasi/{oglasId:int}/razgovor")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ConversationDto>> Open(int oglasId)
        {
            var result = await _conversationService.OpenAsync(oglasId);
            return CreatedAtAction(nameof(GetById), new { id = result.ConversationId }, result);
        }

        [HttpGet("api/oglasi/{oglasId:int}/razgovor")]
        public async Task<ActionResult<ConversationDto>> GetForListing(int oglasId)
        {
            return Ok(await _conversationService.GetForListingAsync(oglasId, CurrentUserId, IsAdmin));
        }

        [HttpGet("api/razgovori/{id:int}")]
        public async Task<ActionResult<ConversationDto>> GetById(int id)
        {
            return Ok(await _conversationService.GetByIdAsync(id, CurrentUserId, IsAdmin));
        }

        [HttpPatch("api/razgovori/{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ConversationDto>> UpdateStatus(int id, UpdateConversationStatusDto dto)
        {
            return Ok(await _conversationService.UpdateStatusAsync(id, dto.Status));
        }
    }
}
