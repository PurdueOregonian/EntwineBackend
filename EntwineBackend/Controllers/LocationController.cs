using EntwineBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Location")]
    [Authorize(Policy = "UserId")]
    public class LocationController : ControllerBase
    {
        private readonly ILogger<LocationController> _logger;
        private readonly ILocationService _locationService;

        public LocationController(ILogger<LocationController> logger, ILocationService locationService)
        {
            _logger = logger;
            _locationService = locationService;
        }

        

        [HttpGet]
        public async Task<IActionResult> GetLocationAsync(
            [FromQuery] string latitude,
            [FromQuery] string longitude
            )
        {
            var location = await _locationService.GetLocation(latitude, longitude);
            if (location == null)
            {
                // TODO better logging of error
                _logger.LogError("Failed to get location for lat: {latitude}, long: {longitude}", latitude, longitude);
                return NotFound();
            }
            return Ok(location);
        }
    }
}