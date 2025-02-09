using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EntwineBackend.PubSub
{
    [Authorize(Policy = "UserInChatCommunity")]
    public class CommunityChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var chatId = Context.GetHttpContext()?.Request.Query["chatId"];

            await Groups.AddToGroupAsync(Context.ConnectionId, $"Community-{chatId!}");

            await base.OnConnectedAsync();
        }
    }
}
