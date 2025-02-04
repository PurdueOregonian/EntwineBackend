using EntwineBackend.DbItems;
using EntwineBackend.Data;

namespace EntwineBackend.Services
{
    public interface IProfileService
    {
        public ProfileData? GetProfile(int userId);
        public Task SaveProfile(int userId, string username, ServiceInputProfileData data);
        public List<ProfileData> SearchUsers(int userId, string searchString);
        public List<ProfileData> SearchProfiles(int userId, SearchProfileParams data);
        public string? GetUsernameFromId(int id);
    }
}
