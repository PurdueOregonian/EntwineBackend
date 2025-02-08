using EntwineBackend.Data;
using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface ICommunityService
    {
        CommunityData? GetCommunityData(int userId);
        Community? GetCommunity(int userId);
    }
}
