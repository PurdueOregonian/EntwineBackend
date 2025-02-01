using EntwineBackend.Services;
using EntwineBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Search")]
    [Authorize(Policy = "UserId")]
    public class SearchController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public SearchController(
            IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return BadRequest("Search string cannot be empty.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var users = await _profileService.SearchUsers(userId, searchString);

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> SearchProfilesAsync([FromBody] SearchProfileParams data)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profiles = await _profileService.SearchProfiles(userId, data);

            return Ok(profiles);
        }
    }
}