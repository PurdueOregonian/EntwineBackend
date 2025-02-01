using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface ICommunityService
    {
        public Task<Community?> GetCommunity(int userId);
    }
}
