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
        private readonly ILocationService _locationService;

        public SearchController(
            IProfileService profileService,
            ILocationService locationService)
        {
            _profileService = profileService;
            _locationService = locationService;
        }

        [HttpGet("Users/Username")]
        public IActionResult SearchUsers([FromQuery] string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return BadRequest("Search string cannot be empty.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var users = _profileService.SearchUsers(userId, searchString);

            return Ok(users);
        }

        [HttpPost("Users/Profile")]
        public IActionResult SearchProfilesAsync([FromBody] SearchProfileParams data)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profiles = _profileService.SearchProfiles(userId, data);

            return Ok(profiles.Select(profile => new UserSearchResult
            {
                Id = profile.Id,
                Username = profile.Username,
            }));
        }
    }
}