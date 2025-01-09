using EntwineBackend.Authentication;
using EntwineBackend.UserId;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EntwineBackend.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> RegisterUser(ValidLoginInfo loginInfo);
        public Task<UserAndErrorMessage> Login(ValidLoginInfo loginInfo);
        public void InvalidateToken(string token);
        public string GenerateJwtToken(ApplicationUser user, DateTime expirationTime);
        public ClaimsPrincipal? ValidateRefreshToken(string token);
        public ApplicationUser? GetUserFromName(string username);
        public string? GetUsernameFromId(int id);
    }
}
