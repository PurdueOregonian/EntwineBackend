using EntwineBackend.Data;

namespace EntwineBackend.Services
{
    public interface ICommunityService
    {
        CommunityData? GetCommunity(int userId);
    }
}
