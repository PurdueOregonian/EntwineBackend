using Friends5___Backend.DbItems;

namespace Friends5___Backend.Services
{
    public interface IProfileService
    {
        public Task<ProfileData?> GetProfile(string username);
        public Task SaveProfile(string username, ReceivedProfileData data);
        public Task<List<ProfileData>> SearchUsers(string username, string searchString);
        public Task<List<ProfileData>> SearchProfiles(string username, SearchProfileData data);
        public Task<string?> GetUsernameFromId(int id);
    }
}
