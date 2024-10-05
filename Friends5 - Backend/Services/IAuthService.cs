using Friends5___Backend.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Friends5___Backend.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> RegisterUser(ValidLoginInfo loginInfo);
        public Task<SuccessAndErrorMessage> Login(ValidLoginInfo loginInfo);
        public void InvalidateToken(string token);
        public string GenerateJwtToken(string username, DateTime expirationTime);
        public ClaimsPrincipal? ValidateRefreshToken(string token);
    }
}
