using EntwineBackend.DbContext;
using Friends5___Backend;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Interest")]
    [Authorize(Policy = "UserId")]
    public class InterestController : ControllerBase
    {
        private readonly EntwineDbContext _dbContext;

        public InterestController(
            EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Category")]
        public IActionResult GetInterestCategories()
        {
            var categories = DbFunctions.GetInterestCategories(_dbContext);

            return Ok(categories);
        }

        [HttpGet]
        public IActionResult GetInterests()
        {
            var interests = DbFunctions.GetInterests(_dbContext);

            return Ok(interests);
        }
    }
}