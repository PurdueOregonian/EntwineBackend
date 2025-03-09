using EntwineBackend.Data;
using EntwineBackend.DbContext;
using EntwineBackend.PubSub;
using EntwineBackend.Services;
using Friends5___Backend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Community")]
    [Authorize(Policy = "UserId")]
    public class CommunityController : ControllerBase
    {
        private readonly EntwineDbContext _dbContext;
        private readonly IHubContext<CommunityChatHub> _communityChatHubContext;

        public CommunityController(
            EntwineDbContext dbContext,
            IHubContext<CommunityChatHub> communityChatHubContext)
        {
            _dbContext = dbContext;
            _communityChatHubContext = communityChatHubContext;
        }

        [HttpGet]
        public IActionResult GetCommunityAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var communityData = DbFunctions.GetCommunityData(_dbContext, userId);
            if (communityData == null)
            {
                return NotFound();
            }
            return Ok(communityData);
        }

        [HttpGet("Chat/{chatId}/Messages")]
        public IActionResult GetMessages(int chatId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!Utils.IsUserInChatCommunity(_dbContext, userId, chatId))
            {
                return Unauthorized();
            }

            var messages = DbFunctions.CommunityGetMessages(_dbContext, chatId);
            return Ok(messages);
        }

        [HttpPost("Chat/{chatId}/Messages")]
        public async Task<IActionResult> SendMessage(int chatId, [FromBody] MessageToSend data)
        {
            if (data.Content is null)
            {
                return BadRequest();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if(!Utils.IsUserInChatCommunity(_dbContext, userId, chatId))
            {
                return Unauthorized();
            }

            var message = await DbFunctions.CommunitySendMessage(_dbContext, chatId, userId, data.Content);
            await _communityChatHubContext.Clients.Group($"Community-{chatId.ToString()}")
                .SendAsync("ReceiveMessage", message);
            return Ok(message);
        }
    }
}