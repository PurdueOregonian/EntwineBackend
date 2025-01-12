using Microsoft.AspNetCore.Authorization;
namespace EntwineBackend.Authentication
{
    public class UserInChatRequirement : IAuthorizationRequirement
    {
        public UserInChatRequirement()
        {
        }
    }
}
