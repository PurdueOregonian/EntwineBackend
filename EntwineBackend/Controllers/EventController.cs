using EntwineBackend.Data;
using EntwineBackend.DbContext;
using Friends5___Backend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Events")]
    [Authorize(Policy = "UserId")]
    public class EventController : ControllerBase
    {
        private readonly EntwineDbContext _dbContext;
        public EventController(EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventData data)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var createdEvent = await DbFunctions.CreateEvent(_dbContext, userId, data);
            if(createdEvent == null)
            {
                return BadRequest("User not in a community. Could not create event.");
            }
            return Ok(createdEvent);
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var events = DbFunctions.GetEvents(_dbContext, userId);
            return Ok(events);
        }
    }
}