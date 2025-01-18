using EntwineBackend.DbItems;
using Friends5___Backend.Data;

namespace EntwineBackend.Services
{
    public interface IProfileService
    {
        public Task<ProfileData?> GetProfile(int userId);
        public Task SaveProfile(int userId, string username, ServiceInputProfileData data);
        public Task<List<ProfileData>> SearchUsers(int userId, string searchString);
        public Task<List<ProfileData>> SearchProfiles(int userId, SearchProfileParams data);
        public Task<string?> GetUsernameFromId(int id);
    }
}
