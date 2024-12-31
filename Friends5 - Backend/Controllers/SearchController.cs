using Friends5___Backend.DbItems;
using Friends5___Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Search")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        IConfiguration _config;
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileService _profileService;

        public SearchController(
            ILogger<ProfileController> logger,
            IConfiguration config,
            IProfileService profileService)
        {
            _logger = logger;
            _config = config;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchString)
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return BadRequest("Search string cannot be empty.");
            }

            var users = await _profileService.SearchUsers(User.Identity.Name, searchString);

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> SearchProfilesAsync([FromBody] SearchProfileData data)
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            var profiles = await _profileService.SearchProfiles(User.Identity.Name, data);

            return Ok(profiles);
        }
    }
}