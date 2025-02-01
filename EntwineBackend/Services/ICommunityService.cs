using EntwineBackend.Data;
using Npgsql;

namespace EntwineBackend.Services
{
    public interface ICommunityService
    {
        public Task<CommunityData?> GetCommunity(int userId);
    }
}
