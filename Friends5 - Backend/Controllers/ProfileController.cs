using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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
                INSERT INTO public.profiles(""Username"", ""DateOfBirth"", ""Gender"")
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
                    cmd.Parameters.AddWithValue("@Gender", data.Gender);
                }
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
            return Ok();
        }
    }
}