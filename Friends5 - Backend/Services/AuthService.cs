using Friends5___Backend.Authentication;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Friends5___Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TokenBlacklist _tokenBlacklist = new();

        public AuthService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IdentityResult> RegisterUser(LoginInfo loginInfo)
        {
            var identityUser = new IdentityUser
            {
                UserName = loginInfo.Username
            };

            try
            {
                var result = await _userManager.CreateAsync(identityUser, loginInfo.Password);
                return result;
            }
            catch (Exception)
            {
                return IdentityResult.Failed(new IdentityError { Code = "CustomErrorCode", Description = "An error occurred while creating the user." });
            }
        }
        public async Task<LoginResult> Login(LoginInfo loginInfo)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginInfo.Username);
                if (user == null)
                {
                    return new LoginResult { Success = false };
                }
                return new LoginResult { Success = await _userManager.CheckPasswordAsync(user, loginInfo.Password) };
            }
            catch(Exception ex)
            {
                return new LoginResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        public void Logout(string token)
        {
            _tokenBlacklist.AddToken(token);
        }
    }
}
