using Lost_Found.DTOs.Message;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lost_Found.Controllers
{
    [ApiController]
    [Authorize]
    public class MessagesController : ApiControllerBase
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("api/razgovori/{razgovorId:int}/poruke")]
        public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetForConversation(int razgovorId)
        {
            return Ok(await _messageService.GetForConversationAsync(razgovorId, CurrentUserId, IsAdmin));
        }

        [HttpPost("api/razgovori/{razgovorId:int}/poruke")]
        public async Task<ActionResult<MessageDto>> Create(int razgovorId, MessageCreateDto dto)
        {
            var result = await _messageService.CreateAsync(razgovorId, CurrentUserId, IsAdmin, dto);
            return CreatedAtAction(nameof(GetForConversation), new { razgovorId }, result);
        }
    }
}
