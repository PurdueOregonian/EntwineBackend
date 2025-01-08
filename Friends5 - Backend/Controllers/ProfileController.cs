using Friends5___Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Profile")]
    [Authorize(Policy = "UserId")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileService _profileService;

        public ProfileController(
            ILogger<ProfileController> logger,
            IProfileService profileService)
        {
            _logger = logger;
            _profileService = profileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwnProfileAsync()
        {
            var username = User.Identity!.Name!.ToString();

            var profile = await _profileService.GetProfile(username);

            if (profile == null)
            {
                return NotFound();
            }

            return Ok(profile);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfileAsync(string username)
        {
            var profile = await _profileService.GetProfile(username);

            if (profile == null)
            {
                return NotFound();
            }

            var dataToReturn = new ProfileSearchReturnData
            {
                Age = profile.DateOfBirth == null ? null : Utils.YearsSince(profile.DateOfBirth.Value),
                Gender = profile.Gender,
                Interests = profile.Interests
            };
            return Ok(dataToReturn);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveProfileAsync([FromBody] ReceivedProfileData data)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                await _profileService.SaveProfile(userId, User.Identity!.Name!, data);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
            return Ok();
        }
    }
}