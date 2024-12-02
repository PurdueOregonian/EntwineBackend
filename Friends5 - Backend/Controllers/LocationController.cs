using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Text.Json;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Location")]
    [Authorize]
    public class LocationController : ControllerBase
    {
        IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILogger<LocationController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationAsync(
            [FromQuery] string latitude,
            [FromQuery] string longitude
            )
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var locationResponse = await httpClient.GetAsync(
                $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key=AIzaSyCdvn48IcClIf3q94R7-8V3JJ-BH5zrBxo");
            var locationString = await locationResponse.Content.ReadAsStringAsync();
            var location = JsonSerializer.Deserialize<object>(locationString, DefaultJsonOptions.Instance);

            return Ok();
        }
    }
}