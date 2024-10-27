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
                command.Parameters.AddWithValue("@Gender", NpgsqlDbType.Array | NpgsqlDbType.Text, data.Gender.ToArray());
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                profiles.Add(new ProfileData
                {
                    Username = reader.GetString(0),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(1)),
                    Gender = reader.GetString(2)
                });
            }
            return Ok(profiles);
        }
    }
}