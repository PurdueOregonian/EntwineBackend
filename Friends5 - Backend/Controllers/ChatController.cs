using Friends5___Backend.DbItems;
using Friends5___Backend.PubSub;
using Friends5___Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Chat")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IProfileService _profileService;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatController(
            IChatService chatService,
            IProfileService profileService,
            IHubContext<ChatHub> chatHubContext)
        {
            _chatService = chatService;
            _profileService = profileService;
            _chatHubContext = chatHubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatData data)
        {
            if (data.UserIds is null)
            {
                return BadRequest();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim.Value);
            var userIds = data.UserIds;
            userIds.Add(userId);
            var chat = await _chatService.CreateChat(userIds);
            if (chat == null)
            {
                return StatusCode(500);
            }
            return Ok(chat);
        }

        [HttpGet]
        public async Task<IActionResult> GetChats()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var chats = await _chatService.GetChats(userId);

            var chatDatas = new List<ChatData>();
            foreach (var chat in chats)
            {
                var otherUsernames = new List<string>();
                foreach (var id in chat.UserIds ?? [])
                {
                    if (id != userId)
                    {
                        var username = await _profileService.GetUsernameFromId(id);
                        if (username != null)
                        {
                            otherUsernames.Add(username);
                        }
                    }
                }
                chatDatas.Add(new ChatData
                {
                    Id = chat.Id,
                    Usernames = otherUsernames
                });
            }

            return Ok(chatDatas);
        }

        [HttpGet("{chatId}/Messages")]
        public async Task<IActionResult> GetMessages([FromRoute] int chatId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var chat = await _chatService.GetChat(chatId);

            if(chat is null)
            {
                return NotFound();
            }

            if(chat.UserIds is null)
            {
                return StatusCode(500);
            }

            var userId = int.Parse(userIdClaim.Value);

            if (!chat.UserIds.Contains(userId))
            {
                // maybe giving away info about which users are in the chat is a bad idea so return NotFound
                return NotFound();
            }

            var messages = await _chatService.GetMessages(chatId);

            return Ok(messages);
        }

        [HttpPost("{chatId}/Messages")]
        public async Task<IActionResult> SendMessage([FromRoute] int chatId, [FromBody] MessageToSend data)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (data.Content is null)
            {
                return BadRequest();
            }

            var chat = await _chatService.GetChat(chatId);
            if (chat is null)
            {
                return NotFound();
            }
            if (chat.UserIds is null)
            {
                return StatusCode(500);
            }
            var userId = int.Parse(userIdClaim.Value);
            if (!chat.UserIds.Contains(userId))
            {
                return NotFound();
            }

            var message = await _chatService.SendMessage(chatId, userId, data.Content);
            await _chatHubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message);

            return Ok(message);
        }
    }
}