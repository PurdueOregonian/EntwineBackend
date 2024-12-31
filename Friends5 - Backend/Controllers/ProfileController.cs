using Friends5___Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Profile")]
    [Authorize]
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
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            var username = User.Identity.Name.ToString();

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
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            var profile = await _profileService.GetProfile(username);

            if (profile == null)
            {
                return NotFound();
            }

            var dataToReturn = new ProfileSearchReturnData
            {
                Age = profile.DateOfBirth == null ? null : Utils.YearsSince(profile.DateOfBirth.Value),
                Gender = profile.Gender
            };
            return Ok(dataToReturn);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveProfileAsync([FromBody] ReceivedProfileData data)
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }
            try
            {
                await _profileService.SaveProfile(User.Identity.Name, data);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
            return Ok();
        }
    }
}