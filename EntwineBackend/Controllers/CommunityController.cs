using EntwineBackend.Data;
using EntwineBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IProfileService _profileService;

        public CommunityController(
            ICommunityService communityService,
            ICommunityChatService communityChatService,
            IProfileService profileService)
        {
            _communityService = communityService;
            _communityChatService = communityChatService;
            _profileService = profileService;
        }

        [HttpGet]
        public IActionResult GetCommunityAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            return Ok(_communityService.GetCommunityData(userId));
        }

        [HttpGet("Chat/{chatId}/Messages")]
        public IActionResult GetMessages(int chatId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            //TODO change when user has community list
            var userLocation = _profileService.GetProfile(userId)?.Location;
            var community = _communityChatService.GetChatCommunity(chatId);

            if (userLocation != community)
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

            //TODO change when user has community list
            var userLocation = _profileService.GetProfile(userId)?.Location;
            var community = _communityChatService.GetChatCommunity(chatId);

            if (userLocation != community)
            {
                return Unauthorized();
            }

            var result = await _communityChatService.SendMessage(userId, chatId, data.Content);
            return Ok(result);
        }
    }
}