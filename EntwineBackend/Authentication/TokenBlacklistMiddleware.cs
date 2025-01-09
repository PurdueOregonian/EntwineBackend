namespace EntwineBackend.Authentication
{
    public class TokenBlacklistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenBlacklist _tokenBlacklist;

        public TokenBlacklistMiddleware(RequestDelegate next, TokenBlacklist tokenBlacklist)
        {
            _next = next;
            _tokenBlacklist = tokenBlacklist;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token) && !_tokenBlacklist.IsTokenValid(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is blacklisted.");
                return;
            }

            await _next(context);
        }
    }
}
