using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Profile")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        IConfiguration _config;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(ILogger<ProfileController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfileAsync()
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            var username = User.Identity.Name.ToString();

            var sql = @"SELECT * FROM public.""Profiles""
                        WHERE ""Profiles"".""Username"" = @Username";

            using var command = dataSource.CreateCommand(sql);

            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var profile = new ProfileData
                {
                    Username = reader.GetString(0),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(1)),
                    Gender = (Gender)reader.GetInt32(2)
                };
                return Ok(profile);
            }
            else
            {
                return Ok(new ProfileData());
            }
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveProfileAsync([FromBody] ReceivedProfileData data)
        {
            if (User.Identity?.Name is null)
            {
                return Unauthorized();
            }

            var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            var username = User.Identity.Name.ToString();

            await using (var cmd = dataSource.CreateCommand(@"
                INSERT INTO public.""Profiles""(""Username"", ""DateOfBirth"", ""Gender"")
                VALUES (@Username, @DateOfBirth, @Gender)
                ON CONFLICT (""Username"") 
                DO UPDATE 
                SET ""DateOfBirth"" = EXCLUDED.""DateOfBirth"", ""Gender"" = EXCLUDED.""Gender"";
            "))
            {
                cmd.Parameters.AddWithValue("@Username", User.Identity.Name);
                if (data.DateOfBirth is not null)
                {
                    cmd.Parameters.AddWithValue("@DateOfBirth", data.DateOfBirth);
                }
                if (data.Gender is not null)
                {
                    cmd.Parameters.AddWithValue("@Gender", (int)data.Gender);
                }
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception e)
                {
                    return StatusCode(500);
                }
            }
            return Ok();
        }
    }
}