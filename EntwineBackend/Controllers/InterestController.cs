using EntwineBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EntwineBackend.Controllers
{
    [ApiController]
    [Route("/Interest")]
    [Authorize(Policy = "UserId")]
    public class InterestController : ControllerBase
    {
        private readonly IInterestService _interestService;

        public InterestController(
            IInterestService interestService)
        {
            _interestService = interestService;
        }

        [HttpGet("Category")]
        public IActionResult GetInterestCategories()
        {
            var categories = _interestService.GetInterestCategories();

            return Ok(categories);
        }

        [HttpGet]
        public IActionResult GetInterests()
        {
            var interests = _interestService.GetInterests();

            return Ok(interests);
        }
    }
}