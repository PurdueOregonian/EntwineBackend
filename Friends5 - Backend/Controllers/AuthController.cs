using Friends5___Backend.Authentication;
using Friends5___Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("/Auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInfo info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var loginResult = await _authService.Login(info);
            if (loginResult.Success)
            {
                var accessToken = _authService.GenerateJwtToken(info.Username, DateTime.Now.AddMinutes(30));
                var refreshToken = _authService.GenerateJwtToken(info.Username, DateTime.Now.AddDays(1));
                Response.Cookies.Append("refresh", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    // SameSite = SameSiteMode.Strict, //TODO make this work
                    Expires = DateTimeOffset.UtcNow.AddHours(1)
                });
                return Ok(new LoginResult
                {
                    AccessToken = accessToken
                });
            }
            if (loginResult.ErrorMessage != null)
            {
                return StatusCode(500);
            }
            return BadRequest("Incorrect username or password.");
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] LoginInfo info)
        {
            var registerResult = await _authService.RegisterUser(info);
            if(registerResult.Succeeded)
            {
                return Ok();
            }
            return BadRequest(registerResult.Errors.First().Description);
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public IActionResult Refresh()
        {
            var refreshToken = Request.Cookies["refresh"];
            if (refreshToken == null)
            {
                return Unauthorized("No refresh token or no username");
            }
            var principal = _authService.ValidateRefreshToken(refreshToken);
            if (principal?.Identity?.Name == null)
            {
                return Unauthorized("Invalid refresh token");
            }
            var username = principal.Identity.Name;
            var newAccessToken = _authService.GenerateJwtToken(username, DateTime.Now.AddMinutes(30));

            return Ok(new { AccessToken = newAccessToken });
        }


        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token not provided.");
            }

            _authService.Logout(token);

            return Ok(new { message = "Logged out successfully" });
        }
    }
}