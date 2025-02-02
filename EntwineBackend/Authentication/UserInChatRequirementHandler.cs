using EntwineBackend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace EntwineBackend.Authentication
{
    public class UserInChatRequirementHandler : AuthorizationHandler<UserInChatRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatService _chatService;

        public UserInChatRequirementHandler(IHttpContextAccessor httpContextAccessor, IChatService chatService)
        {
            _httpContextAccessor = httpContextAccessor;
            _chatService = chatService;
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

            var chat = _chatService.GetChat(chatId);

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
