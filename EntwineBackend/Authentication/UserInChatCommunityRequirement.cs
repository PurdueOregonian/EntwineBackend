using Microsoft.AspNetCore.Authorization;
namespace EntwineBackend.Authentication
{
    public class UserInChatCommunityRequirement : IAuthorizationRequirement
    {
        public UserInChatCommunityRequirement()
        {
        }
    }
}
