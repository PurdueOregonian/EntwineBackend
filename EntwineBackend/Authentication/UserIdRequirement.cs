using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EntwineBackend.Authentication
{
    public class UserIdRequirement : AuthorizationHandler<UserIdRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserIdRequirement requirement)
        {
            if (context.User.Identity?.Name is null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            if (int.TryParse(userIdClaim.Value, out var userId))
            {
                if (context.Resource is HttpContext httpContext)
                {
                    httpContext.Items["UserId"] = userId;
                }
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
