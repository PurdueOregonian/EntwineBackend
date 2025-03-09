using EntwineBackend.PubSub;
using EntwineBackend.Services;
using EntwineBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using EntwineBackend.DbContext;
using Friends5___Backend;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Chat")]
    [Authorize(Policy = "UserId")]
    public class ChatController : ControllerBase
    {
        private readonly EntwineDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public ChatController(
            EntwineDbContext dbContext,
            IAuthService authService,
            IHubContext<ChatHub> chatHubContext)
        {
            _dbContext = dbContext;
            _authService = authService;
            _chatHubContext = chatHubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatData data)
        {
            if (data.UserIds is null)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userIds = data.UserIds;
            userIds.Add(userId);
            var chat = await DbFunctions.CreateChat(_dbContext, userIds);
            if (chat == null)
            {
                return StatusCode(500);
            }
            return Ok(chat);
        }

        [HttpGet]
        public IActionResult GetChats()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var chats = DbFunctions.GetChats(_dbContext, userId);

            var chatDatas = new List<ChatData>();
            foreach (var chat in chats)
            {
                var otherUsernames = new List<string>();
                foreach (var id in chat.UserIds ?? [])
                {
                    if (id != userId)
                    {
                        var username = _authService.GetUsernameFromId(id);
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
        public IActionResult GetMessages([FromRoute] int chatId)
        {
            var chat = DbFunctions.GetChat(_dbContext, chatId);

            if(chat is null)
            {
                return NotFound();
            }

            if(chat.UserIds is null)
            {
                return StatusCode(500);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!chat.UserIds.Contains(userId))
            {
                // maybe giving away info about which users are in the chat is a bad idea so return NotFound
                return NotFound();
            }

            var messages = DbFunctions.GetMessages(_dbContext, chatId);

            return Ok(messages);
        }

        [HttpPost("{chatId}/Messages")]
        public async Task<IActionResult> SendMessage([FromRoute] int chatId, [FromBody] MessageToSend data)
        {
            if (data.Content is null)
            {
                return BadRequest();
            }

            var chat = DbFunctions.GetChat(_dbContext, chatId);
            if (chat is null)
            {
                return NotFound();
            }
            if (chat.UserIds is null)
            {
                return StatusCode(500);
            }
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (!chat.UserIds.Contains(userId))
            {
                return NotFound();
            }

            var message = await DbFunctions.SendMessage(_dbContext, chatId, userId, data.Content);
            await _chatHubContext.Clients.Group(chatId.ToString()).SendAsync("ReceiveMessage", message);

            return Ok(message);
        }
    }
}