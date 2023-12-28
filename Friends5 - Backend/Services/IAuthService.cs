using Friends5___Backend.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Friends5___Backend.Services
{
    public interface IAuthService
    {
        public Task<IdentityResult> RegisterUser(LoginInfo loginInfo);
        public Task<bool> Login(LoginInfo loginInfo);
    }
}
