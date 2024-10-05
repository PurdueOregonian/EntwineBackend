using Friends5___Backend.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Friends5___Backend_Tests
{
    public class TokenBlacklistTests
    {
        private readonly TokenBlacklist _tokenBlacklist = new TokenBlacklist();

        [Fact]
        public void ExpiredTokenNotValid()
        {
            var token = new JwtSecurityToken(
                "AnIssuer",
                "AnAudience",
                [],
                expires: new DateTime(2024, 1, 1)
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _tokenBlacklist.AddToken(tokenString);
            Assert.False(_tokenBlacklist.IsTokenValid(tokenString));
        }

        [Fact]
        public void AddedTokenNotValid()
        {
            var token = new JwtSecurityToken(
                "AnIssuer",
                "AnAudience",
                [],
                expires: DateTime.Now.AddHours(1)
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _tokenBlacklist.AddToken(tokenString);
            Assert.False(_tokenBlacklist.IsTokenValid(tokenString));
        }

        [Fact]
        public void TokenNotAdded_IsValid()
        {
            var token = new JwtSecurityToken(
                "AnIssuer",
                "AnAudience",
                [],
                expires: DateTime.Now.AddHours(1)
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Assert.True(_tokenBlacklist.IsTokenValid(tokenString));
        }
    }
}