using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Security.Principal;

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
                INSERT INTO public.profiles(""Name"", ""Interest"", ""Username"")
                VALUES (@Name, @Interest, @Username)
                ON CONFLICT (""Username"") 
                DO UPDATE 
                SET ""Name"" = EXCLUDED.""Name"", ""Interest"" = EXCLUDED.""Interest"";
            "))
            {
                cmd.Parameters.AddWithValue("@Name", data.Name);
                cmd.Parameters.AddWithValue("@Interest", data.Interest);
                cmd.Parameters.AddWithValue("@Username", User.Identity.Name);
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                { }
            }
            return Ok();
        }
    }
}