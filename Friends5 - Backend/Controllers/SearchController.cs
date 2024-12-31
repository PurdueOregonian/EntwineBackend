using Friends5___Backend.DbItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Search")]
    [Authorize]
    public class SearchController : ControllerBase
    {
        IConfiguration _config;
        private readonly ILogger<ProfileController> _logger;

        public SearchController(ILogger<ProfileController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers([FromQuery] string searchString)
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(searchString))
            {
                return BadRequest("Search string cannot be empty.");
            }

            var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            var users = new List<ProfileData>();

            var sql = @"SELECT * FROM public.""AspNetUsers"" WHERE ""UserName"" ILIKE @SearchString";

            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@SearchString", $"%{searchString}%");

            var username = User.Identity.Name.ToString();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new ProfileData
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1)
                };
                if (user.Username != username)
                {
                    users.Add(user);
                }
            }

            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> SearchProfilesAsync([FromBody] SearchProfileData data)
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            var profiles = new List<ProfileData>();

            var query = new List<string>
            {
                @"SELECT * FROM public.""Profiles"" WHERE",
                @"DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) >= @MinAge",
                @"AND DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) <= @MaxAge"
            };

            if (data.Gender != null && data.Gender.Count > 0)
            {
                query.Add(@"AND ""Profiles"".""Gender"" = ANY(@Gender)");
            }

            var sql = string.Join(" ", query);

            using var command = dataSource.CreateCommand(sql);

            command.Parameters.AddWithValue("@MinAge", data.MinAge);
            command.Parameters.AddWithValue("@MaxAge", data.MaxAge);

            if (data.Gender != null && data.Gender.Count > 0)
            {
                command.Parameters.AddWithValue("@Gender", NpgsqlDbType.Array | NpgsqlDbType.Integer, data.Gender.Select(g => (int)g).ToArray());
            }
            var username = User.Identity.Name.ToString();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var profile = new ProfileData
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    Gender = (Gender)reader.GetInt32(3)
                };
                if (profile.Username != username)
                {
                    profiles.Add(profile);
                }
            }
            return Ok(profiles);
        }
    }
}