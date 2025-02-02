using EntwineBackend.DbContext;
using EntwineBackend.DbItems;
using Npgsql;
using System.Text;
using System.Text.Json.Nodes;

namespace EntwineBackend.Services
{
    public class LocationService : ILocationService
    {
        IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private readonly EntwineDbContext _dbContext;

        public LocationService(IHttpClientFactory httpClientFactory, EntwineDbContext dbContext)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = ReadApiKeyFromFile("./ApiKey.txt");
            _dbContext = dbContext;
        }

        private string ReadApiKeyFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath).Trim();
            }
            throw new FileNotFoundException("API key file not found.", filePath);
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

        public async Task<int> GetLocationId(InputLocation location)
        {
            var matchingLocation = _dbContext.Locations.FirstOrDefault(
                l => l.City == location.City && l.Country == location.Country && l.State == location.State);
            if (matchingLocation != null)
            {
                return matchingLocation.Id;
            }
            else
            {
                // Create new location and return Id
                // We also need to add a Community
                var newLocation = new Location
                {
                    City = location.City,
                    Country = location.Country,
                    State = location.State
                };
                _dbContext.Locations.Add(newLocation);
                await _dbContext.SaveChangesAsync();

                // The newLocation.Id will be populated with the generated ID
                await CreateNewCommunity(newLocation.Id);

                return newLocation.Id;
            }
        }

        public Location? GetLocationById(int locationId)
        {
            return _dbContext.Locations.FirstOrDefault(l => l.Id == locationId);
        }

        private async Task CreateNewCommunity(int locationId)
        {
            var community = new Community
            {
                Location = locationId,
                UserIds = []
            };
            _dbContext.Communities.Add(community);
            await _dbContext.SaveChangesAsync();

            // Now we need to create the default chats for the community. For each interest category, create a chat
            var defaultChats = Constants.InterestCategories.Select(category =>
                new CommunityChat
                {
                    Name = category.Name,
                    Community = community.Id
                }
            ).ToList();
            _dbContext.CommunityChats.AddRange(defaultChats);
            await _dbContext.SaveChangesAsync();
        }
    }
}
