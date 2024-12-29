using Friends5___Backend.Authentication;
using Friends5___Backend.UserId;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Friends5___Backend.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> RegisterUser(ValidLoginInfo loginInfo);
        public Task<UserAndErrorMessage> Login(ValidLoginInfo loginInfo);
        public void InvalidateToken(string token);
        public string GenerateJwtToken(ApplicationUser user, DateTime expirationTime);
        public ClaimsPrincipal? ValidateRefreshToken(string token);
        public ApplicationUser? GetUserFromName(string username);
    }
}
