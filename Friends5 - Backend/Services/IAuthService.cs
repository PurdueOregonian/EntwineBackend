using Friends5___Backend.Authentication;

namespace Friends5___Backend.Services
{
    public interface IAuthService
    {
        public Task<bool> RegisterUser(LoginInfo loginInfo);
        public Task<bool> Login(LoginInfo loginInfo);
    }
}
