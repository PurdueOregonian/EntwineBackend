using EntwineBackend.DbItems;

namespace Friends5___Backend.Services
{
    public interface ICommunityService
    {
        public Task<Community?> GetCommunity(int userId);
    }
}
