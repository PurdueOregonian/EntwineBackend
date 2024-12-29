using Friends5___Backend.Authentication;
using Friends5___Backend.UserId;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Friends5___Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenBlacklist _tokenBlacklist;
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public AuthService(IConfiguration config, UserManager<ApplicationUser> userManager, TokenBlacklist tokenBlacklist)
        {
            _config = config;
            _userManager = userManager;
            _tokenBlacklist = tokenBlacklist;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("Jwt:Key")!));
        }
        public async Task<IdentityResult> RegisterUser(ValidLoginInfo loginInfo)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = loginInfo.Username
            };

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, loginInfo.Password);
                return result;
            }
            catch(Exception)
            {
                return IdentityResult.Failed(new IdentityError { Code = "CustomErrorCode", Description = "An error occurred while creating the user." });
            }
        }
        public async Task<UserAndErrorMessage> Login(ValidLoginInfo loginInfo)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginInfo.Username);
                if (user == null)
                {
                    return new UserAndErrorMessage();
                }
                var passwordValid = await _userManager.CheckPasswordAsync(user, loginInfo.Password);
                return new UserAndErrorMessage { User = passwordValid ? user : null };
            }
            catch(Exception ex)
            {
                return new UserAndErrorMessage { ErrorMessage = ex.Message };
            }
        }

        public void InvalidateToken(string token)
        {
            _tokenBlacklist.AddToken(token);
        }

        public string GenerateJwtToken(ApplicationUser user, DateTime expirationTime)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256); //TODO can 512 work?

            var token = new JwtSecurityToken(
                _config.GetValue<string>("Jwt:Issuer"),
                _config.GetValue<string>("Jwt:Audience"),
                claims,
                expires: expirationTime,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateRefreshToken(string token)
        {
            if (!_tokenBlacklist.IsTokenValid(token))
            {
                return null;
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken && jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
                {
                    return principal;
                }
            }
            catch
            {
                // Invalid token
            }

            return null;
        }

        public ApplicationUser? GetUserFromName(string username)
        {
            return _userManager.FindByNameAsync(username).Result;
        }
    }
}
