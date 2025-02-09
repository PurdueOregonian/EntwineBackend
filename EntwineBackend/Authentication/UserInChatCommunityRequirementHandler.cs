using EntwineBackend.DbContext;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace EntwineBackend.Authentication
{
    public class UserInChatCommunityRequirementHandler : AuthorizationHandler<UserInChatCommunityRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EntwineDbContext _dbContext;

        public UserInChatCommunityRequirementHandler(
            IHttpContextAccessor httpContextAccessor,
            EntwineDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserInChatCommunityRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (!int.TryParse(httpContext?.Request.Query["chatId"].ToString(), out var chatId))
            {
                context.Fail();
                return;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Fail();
                return;
            }

            var userId = int.Parse(userIdClaim.Value);
            if (Utils.IsUserInChatCommunity(_dbContext, userId, chatId))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
