using Friends5___Backend.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Friends5___Backend.Services
{
    public class AuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly PasswordHasher<string> _passwordHasher;

        public AuthService(UserManager<IdentityUser> userManager, PasswordHasher<string> passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }
        public async Task<bool> RegisterUser(LoginInfo loginInfo)
        {
            var identityUser = new IdentityUser
            {
                UserName = loginInfo.Username,
                PasswordHash = _passwordHasher.HashPassword(loginInfo.Username, loginInfo.Password)
            };

            var result = await _userManager.CreateAsync(identityUser, loginInfo.Password);
            return result.Succeeded;
        }
        public async Task<bool> Login(LoginInfo loginInfo)
        {
            var user = await _userManager.FindByNameAsync(loginInfo.Username);
            if (user == null) {
                return false;
            }
            return await _userManager.CheckPasswordAsync(user, loginInfo.Password);
        }
    }
}
