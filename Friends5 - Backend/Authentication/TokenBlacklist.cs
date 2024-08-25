using System.IdentityModel.Tokens.Jwt;

namespace Friends5___Backend.Authentication
{
    public class TokenBlacklist
    {
        private readonly Dictionary<string, DateTime> _blacklist = new Dictionary<string, DateTime>();

        public void AddToken(string token)
        {
            var expiry = GetTokenExpiry(token);
            _blacklist[token] = expiry;
        }

        public bool IsTokenValid(string token)
        {
            if (_blacklist.TryGetValue(token, out var expiry))
            {
                if (DateTime.UtcNow > expiry)
                {
                    _blacklist.Remove(token);
                    return false;
                }
                return false;
            }
            return true;
        }

        public void RemoveExpiredTokens()
        {
            var now = DateTime.UtcNow;
            var expiredTokens = _blacklist.Where(kv => kv.Value < now).Select(kv => kv.Key).ToList();

            foreach (var token in expiredTokens)
            {
                _blacklist.Remove(token);
            }
        }

        private DateTime GetTokenExpiry(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new ArgumentException("Invalid token");

            var expiryUnix = long.Parse(jwtToken.Claims.First(x => x.Type == "exp").Value);
            var expiryDateTimeUtc = DateTimeOffset.FromUnixTimeSeconds(expiryUnix).UtcDateTime;

            return expiryDateTimeUtc;
        }
    }
}
