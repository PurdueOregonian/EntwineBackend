using Friends5___Backend.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Friends5___Backend.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> RegisterUser(LoginInfo loginInfo);
        public Task<LoginResult> Login(LoginInfo loginInfo);
        public void Logout(string token);
        public string GenerateJwtToken(string username);
        public ClaimsPrincipal ValidateRefreshToken(string token);
        public string GenerateRefreshToken();
    }
}
