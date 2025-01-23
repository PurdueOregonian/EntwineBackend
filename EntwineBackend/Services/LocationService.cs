using EntwineBackend.DbItems;
using Npgsql;
using System.Text.Json.Nodes;

namespace Friends5___Backend.Services
{
    public class LocationService : ILocationService
    {
        IHttpClientFactory _httpClientFactory;
        private readonly string _apiKey;
        private string _connectionString;

        public LocationService(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClientFactory = httpClientFactory;
            _apiKey = ReadApiKeyFromFile("./ApiKey.txt");
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
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
            using var dataSource = NpgsqlDataSource.Create(_connectionString);
            var sql = @"SELECT ""Id"" FROM public.""Locations""
                        WHERE ""City"" = @City AND ""Country"" = @Country AND ""State"" = @State";
            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@City", location.City);
            command.Parameters.AddWithValue("@Country", location.Country);
            command.Parameters.AddWithValue("@State", location.State);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return reader.GetInt32(0);
            }
            else
            {
                // Create new location and return Id
                // We also need to add a Community
                int locationId;
                sql = @"INSERT INTO public.""Locations"" (""City"", ""Country"", ""State"")
                        VALUES (@City, @Country, @State) RETURNING ""Id""";
                using var insertCommand = dataSource.CreateCommand(sql);
                insertCommand.Parameters.AddWithValue("@City", location.City);
                insertCommand.Parameters.AddWithValue("@Country", location.Country);
                insertCommand.Parameters.AddWithValue("@State", location.State);
                using var insertReader = insertCommand.ExecuteReader();
                if (await insertReader.ReadAsync())
                {
                    locationId = insertReader.GetInt32(0);
                }
                else
                {
                    throw new Exception("Failed to insert location");
                }

                sql = @"INSERT INTO public.""Communities"" (""Location"")
                VALUES (@Location)";
                using var communityCommand = dataSource.CreateCommand(sql);
                communityCommand.Parameters.AddWithValue("@Location", locationId);
                await communityCommand.ExecuteNonQueryAsync();

                return locationId;
            }
        }

        public async Task<Location?> GetLocationById(int locationId)
        {
            using var dataSource = NpgsqlDataSource.Create(_connectionString);
            var sql = @"SELECT ""City"", ""Country"", ""State"" FROM public.""Locations""
                        WHERE ""Id"" = @Id";
            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@Id", locationId);
            using var reader = command.ExecuteReader();
            if (await reader.ReadAsync())
            {
                return new Location
                {
                    City = reader.GetString(0),
                    Country = reader.GetString(1),
                    State = reader.GetString(2)
                };
            }
            else
            {
                return null;
            }
        }
    }
}
