using EntwineBackend.DbItems;
using EntwineBackend.Services;
using EntwineBackend.Data;
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
        public IActionResult GetOwnProfileAsync()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profile = _profileService.GetProfile(userId);

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
                Location = profile.Location == null ? null : _locationService.GetLocationById(profile.Location.Id)
            };

            return Ok(ownProfileReturnData);
        }

        [HttpGet("{userId}")]
        public IActionResult GetProfileAsync(int userId)
        {
            var profile = _profileService.GetProfile(userId);

            if (profile == null)
            {
                return NotFound();
            }

            var dataToReturn = new OtherProfileReturnData
            {
                Username = profile.Username,
                Age = profile.DateOfBirth == null ? null : Utils.YearsSince(profile.DateOfBirth.Value),
                Gender = profile.Gender,
                Interests = profile.Interests,
                Location = profile.Location == null ? null : _locationService.GetLocationById(profile.Location.Id)
            };
            return Ok(dataToReturn);
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveProfileAsync([FromBody] InputProfileData data)
        {
            var location = data.Location;

            var serviceInputProfileData = new ServiceInputProfileData
            {
                DateOfBirth = data.DateOfBirth,
                Gender = data.Gender,
                Interests = data.Interests,
                Location = location is null ? null : await _locationService.GetLocationWithId(location)
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