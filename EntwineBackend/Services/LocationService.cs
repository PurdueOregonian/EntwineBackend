using EntwineBackend.DbContext;
using EntwineBackend.DbItems;
using System.Text.Json.Nodes;

namespace EntwineBackend.Services
{
    public class LocationService : ILocationService
    {
        IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;

        public LocationService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = GetApiKey("./ApiKey.txt", config);
        }

        private string GetApiKey(string filePath, IConfiguration config)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath).Trim();
            }
            var apiKey = config.GetValue<string>("ApiKey")!;
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new FileNotFoundException("API key not found in file or configuration.");
            }
            return apiKey;
        }

        public async Task<Location?> GetLocation(
            string latitude,
            string longitude
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
                        return new Location { City = city, Country = country, State = state };
                    }
                }

                return null;
            }

            return null;
        }
    }
}
