using Friends5___Backend.Authentication;
using Friends5___Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Friends5___Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IConfiguration config, IAuthService authService)
        {
            _config = config;
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
            if (await _authService.Login(info))
            {
                var tokenString = GenerateJwtToken(info);
                return Ok(tokenString);
            }
            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<bool> RegisterUser([FromBody] LoginInfo info)
        {
            return await _authService.RegisterUser(info);
        }

        private string GenerateJwtToken(LoginInfo info)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, info.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:Key")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(30); // Token expiration time

            var token = new JwtSecurityToken(
                _config.GetValue<string>("Jwt:Issuer"),
                _config.GetValue<string>("Jwt:Audience"),
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}