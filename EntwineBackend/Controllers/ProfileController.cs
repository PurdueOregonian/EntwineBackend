using EntwineBackend.DbItems;
using EntwineBackend.Services;
using EntwineBackend.Data;
using EntwineBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Profile")]
    [Authorize(Policy = "UserId")]
    public class ProfileController : ControllerBase
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileService _profileService;
        private readonly ILocationService _locationService;

        public ProfileController(
            ILogger<ProfileController> logger,
            IProfileService profileService,
            ILocationService locationService)
        {
            _logger = logger;
            _profileService = profileService;
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOwnProfileAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profile = await _profileService.GetProfile(userId);

            if (profile == null)
            {
                return NotFound();
            }

            var ownProfileReturnData = new OwnProfileReturnData
            {
                Id = profile.Id,
                Username = profile.Username,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Interests = profile.Interests,
                Location = profile.Location == null ? null : await _locationService.GetLocationById(profile.Location.Value)
            };

            return Ok(ownProfileReturnData);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfileAsync(int userId)
        {
            var profile = await _profileService.GetProfile(userId);

            if (profile == null)
            {
                return NotFound();
            }

            var dataToReturn = new ProfileSearchReturnData
            {
                Username = profile.Username,
                Age = profile.DateOfBirth == null ? null : Utils.YearsSince(profile.DateOfBirth.Value),
                Gender = profile.Gender,
                Interests = profile.Interests,
                Location = profile.Location == null ? null : await _locationService.GetLocationById(profile.Location.Value)
            };
            return Ok(dataToReturn);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveProfileAsync([FromBody] InputProfileData data)
        {
            var location = data.Location;
            int? locationId = null;
            if (location != null)
            {
                locationId = await _locationService.GetLocationId(location);
            }

            var serviceInputProfileData = new ServiceInputProfileData
            {
                DateOfBirth = data.DateOfBirth,
                Gender = data.Gender,
                Interests = data.Interests,
                Location = locationId
            };

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                await _profileService.SaveProfile(userId, User.Identity!.Name!, serviceInputProfileData);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok();
        }
    }
}