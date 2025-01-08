using Friends5___Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friends5___Backend.Controllers
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
        public async Task<IActionResult> GetInterestCategories()
        {
            var categories = await _interestService.GetInterestCategories();

            return Ok(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetInterests()
        {
            var interests = await _interestService.GetInterests();

            return Ok(interests);
        }
    }
}