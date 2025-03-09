using EntwineBackend.DbContext;
using Friends5___Backend;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace EntwineBackend.Authentication
{
    public class UserInChatRequirementHandler : AuthorizationHandler<UserInChatRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EntwineDbContext _dbContext;

        public UserInChatRequirementHandler(IHttpContextAccessor httpContextAccessor, EntwineDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserInChatRequirement requirement)
        {
            if (context.User.Identity?.Name is null)
            {
                context.Fail();
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                context.Fail();
                return;
            }

            if (!int.TryParse(httpContext.Request.Query["chatId"].ToString(), out var chatId))
            {
                context.Fail();
                return;
            }

            var chat = DbFunctions.GetChat(_dbContext, chatId);

            if (chat is null || chat.UserIds is null)
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
            if (!chat.UserIds.Contains(userId))
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }
    }
}
