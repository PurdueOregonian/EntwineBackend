using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface IProfileService
    {
        public Task<ProfileData?> GetProfile(int userId);
        public Task SaveProfile(int userId, string username, ReceivedProfileData data);
        public Task<List<ProfileData>> SearchUsers(int userId, string searchString);
        public Task<List<ProfileData>> SearchProfiles(int userId, SearchProfileData data);
        public Task<string?> GetUsernameFromId(int id);
    }
}
