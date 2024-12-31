using Friends5___Backend.DbItems;

namespace Friends5___Backend.Services
{
    public interface IProfileService
    {
        public Task<ProfileData?> GetProfile(string username);
        public Task SaveProfile(string username, ReceivedProfileData data);
    }
}
