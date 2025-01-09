using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Location")]
    [Authorize(Policy = "UserId")]
    public class LocationController : ControllerBase
    {
        IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly ILogger<LocationController> _logger;

        public LocationController(ILogger<LocationController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _apiKey = ReadApiKeyFromFile("./ApiKey.txt");
        }

        private string ReadApiKeyFromFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                return System.IO.File.ReadAllText(filePath).Trim();
            }
            throw new FileNotFoundException("API key file not found.", filePath);
        }

        [HttpGet]
        public async Task<IActionResult> GetLocationAsync(
            [FromQuery] string latitude,
            [FromQuery] string longitude
            )
        {
            var httpClient = _httpClientFactory.CreateClient();
            var locationResponse = await httpClient.GetAsync(
                $"https://maps.googleapis.com/maps/api/geocode/json?latlng={latitude},{longitude}&key={_apiKey}");

            if (locationResponse.IsSuccessStatusCode)
            {
                string json = await locationResponse.Content.ReadAsStringAsync();
                JsonNode? data = JsonNode.Parse(json);

                foreach (var result in data["results"].AsArray())
                {
                    string? city = null;
                    string? country = null;
                    string? state = null;

                    foreach (var component in result["address_components"].AsArray())
                    {
                        var componentTypes = component?["types"]?.AsArray();
                        if (componentTypes != null)
                        {
                            if (componentTypes.ToString().Contains("locality"))
                            {
                                city = component?["long_name"]?.ToString();
                            }
                            else if (componentTypes.ToString().Contains("country"))
                            {
                                country = component?["long_name"]?.ToString();
                            }
                            else if (componentTypes.ToString().Contains("administrative_area_level_1"))
                            {
                                state = component?["long_name"]?.ToString();
                            }
                        }
                    }

                    if (city != null && country != null)
                    {
                        return Ok(new { City = city, Country = country, State = state });
                    }
                }

                return NotFound("City not found");
            }

            return StatusCode(500, new { Message = "Failed to fetch geocoding data." });
        }
    }
}