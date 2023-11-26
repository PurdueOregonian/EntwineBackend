using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [HttpPost("/SaveProfile")]
        public async Task<IActionResult> SaveProfileAsync([FromBody] ProfileData data)
        {
            var connectionString = _config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            await using var dataSource = NpgsqlDataSource.Create(connectionString);

            await using (var cmd = dataSource.CreateCommand("INSERT INTO public.profiles(\"Name\", \"Interest\") VALUES (@Name, @Interest)"))
            {
                cmd.Parameters.AddWithValue("@Name", data.Name);
                cmd.Parameters.AddWithValue("@Interest", data.Interest);
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