using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EntwineBackend.PubSub
{
    [Authorize(Policy = "UserInChat")]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var chatId = Context.GetHttpContext()?.Request.Query["chatId"];

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId!);

            await base.OnConnectedAsync();
        }
    }
}
