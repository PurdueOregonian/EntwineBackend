using EntwineBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EntwineBackend.PubSub
{
    [Authorize(Policy = "UserId")]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                Context.Abort();
                return;
            }

            if(!int.TryParse(Context.GetHttpContext()?.Request.Query["chatId"].ToString(), out var chatId))
            {
                Context.Abort();
                return;
            }

            var chat = await _chatService.GetChat(chatId);

            if (chat is null || chat.UserIds is null)
            {
                Context.Abort();
                return;
            }

            var userId = int.Parse(userIdClaim.Value);

            if (!chat.UserIds.Contains(userId))
            {
                Context.Abort();
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId.ToString());

            await base.OnConnectedAsync();
        }
    }
}
