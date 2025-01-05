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
            if(info.Username is null || info.Password is null)
            {
                return BadRequest("Username or password is null.");
            }
            var validLoginInfo = new ValidLoginInfo
            {
                Username = info.Username,
                Password = info.Password
            };
            var loginResult = await _authService.Login(validLoginInfo);
            if (loginResult.User != null)
            {
                var accessToken = _authService.GenerateJwtToken(loginResult.User, DateTime.Now.AddMinutes(30));
                var refreshToken = _authService.GenerateJwtToken(loginResult.User, DateTime.Now.AddDays(1));
                Response.Cookies.Append("refresh", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None, //TODO make this work
                    Expires = DateTime.Now.AddDays(1)
                });
                return Ok(new LoginResult
                {
                    UserId = loginResult.User.Id,
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
            if(info.Username is null || info.Password is null)
            {
                return BadRequest("Username or password is null.");
            }
            var validLoginInfo = new ValidLoginInfo
            {
                Username = info.Username,
                Password = info.Password,
            };
            var registerResult = await _authService.RegisterUser(validLoginInfo);
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
                return NoContent();
            }
            var principal = _authService.ValidateRefreshToken(refreshToken);
            if (principal?.Identity?.Name == null)
            {
                return NoContent();
            }
            var username = principal.Identity.Name;

            var user = _authService.GetUserFromName(username);
            if(user == null)
            {
                return NoContent();
            }

            var newAccessToken = _authService.GenerateJwtToken(user, DateTime.Now.AddMinutes(30));

            return Ok(new RefreshResult { Username = username, AccessToken = newAccessToken, UserId = user.Id });
        }


        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            var refreshToken = Request.Cookies["refresh"];
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token not provided.");
            }

            _authService.InvalidateToken(token);
            if (refreshToken != null)
            {
                _authService.InvalidateToken(refreshToken);
            }

            Response.Cookies.Append("refresh", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None, //TODO make this work
                Expires = DateTime.Now.AddDays(-1)
            });

            return Ok(new { message = "Logged out successfully" });
        }
    }
}