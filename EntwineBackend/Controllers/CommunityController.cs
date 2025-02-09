using EntwineBackend.Data;
using EntwineBackend.DbContext;
using EntwineBackend.PubSub;
using EntwineBackend.Services;
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
        private readonly ICommunityService _communityService;
        private readonly ICommunityChatService _communityChatService;
        private readonly EntwineDbContext _dbContext;
        private readonly IHubContext<CommunityChatHub> _communityChatHubContext;

        public CommunityController(
            ICommunityService communityService,
            ICommunityChatService communityChatService,
            EntwineDbContext dbContext,
            IHubContext<CommunityChatHub> communityChatHubContext)
        {
            _communityService = communityService;
            _communityChatService = communityChatService;
            _dbContext = dbContext;
            _communityChatHubContext = communityChatHubContext;
        }

        [HttpGet]
        public IActionResult GetCommunityAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var communityData = _communityService.GetCommunityData(userId);
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

            var messages = _communityChatService.GetMessages(chatId);
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

            var message = await _communityChatService.SendMessage(userId, chatId, data.Content);
            await _communityChatHubContext.Clients.Group($"Community-{chatId.ToString()}")
                .SendAsync("ReceiveMessage", message);
            return Ok(message);
        }
    }
}