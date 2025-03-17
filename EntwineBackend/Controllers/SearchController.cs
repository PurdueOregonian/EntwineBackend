using EntwineBackend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EntwineBackend.DbContext;
using Friends5___Backend;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Search")]
    [Authorize(Policy = "UserId")]
    public class SearchController : ControllerBase
    {
        private readonly EntwineDbContext _dbContext;

        public SearchController(
            EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Users/Username")]
        public IActionResult SearchUsers([FromQuery] string searchString)
        {
            if (string.IsNullOrWhiteSpace(searchString))
            {
                return BadRequest("Search string cannot be empty.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var users = DbFunctions.SearchUsers(_dbContext, userId, searchString);

            return Ok(users);
        }

        [HttpPost("Users/Profile")]
        public IActionResult SearchProfilesAsync([FromBody] SearchProfileParams data)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var profiles = DbFunctions.SearchProfiles(_dbContext, userId, data);

            return Ok(profiles.Select(profile => new UserSearchResult
            {
                Id = profile.Id,
                Username = profile.Username,
            }));
        }
    }
}