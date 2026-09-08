using System.Security.Claims;
using Lost_Found.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Lost_Found.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IConversationService _conversationService;

        public ChatHub(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public static string ConversationGroup(int conversationId) => $"conversation-{conversationId}";
        public static string UserGroup(int userId) => $"user-{userId}";

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(CurrentUserId));
            await base.OnConnectedAsync();
        }

        public async Task JoinConversation(int conversationId)
        {
            await _conversationService.EnsureParticipantAsync(conversationId, CurrentUserId, IsAdmin);
            await Groups.AddToGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
        }

        public async Task LeaveConversation(int conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ConversationGroup(conversationId));
        }

        private int CurrentUserId => int.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin => Context.User!.IsInRole("Admin");
    }
}
