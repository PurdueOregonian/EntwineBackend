using EntwineBackend.Data;
using EntwineBackend.DbContext;
using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public class CommunityService : ICommunityService
    {
        private readonly EntwineDbContext _dbContext;

        public CommunityService(
            EntwineDbContext entwineDbContext)
        {
            _dbContext = entwineDbContext;
        }

        public CommunityData? GetCommunityData(int userId)
        {
            var profile = _dbContext.Profiles.FirstOrDefault(p => p.Id == userId);
            if (profile == null || profile.Location == null)
            {
                return null;
            }
            var userLocationId = profile.Location;
            var location = _dbContext.Locations.FirstOrDefault(l => l.Id == userLocationId);

            var community = _dbContext.Communities.FirstOrDefault(c => c.Location == userLocationId);

            if(location == null || community == null)
            {
                return null;
            }

            var communityChats = _dbContext.CommunityChats
                .Where(chat => chat.Community == community.Id)
                .Select(chat => new CommunityChatData
                {
                    Id = chat.Id,
                    Name = chat.Name
                })
                .ToList();

            return new CommunityData
            {
                State = location.State,
                City = location.City,
                Country = location.Country,
                UserIds = community.UserIds,
                Chats = communityChats
            };
        }

        public Community? GetCommunity(int userId)
        {
            var profile = _dbContext.Profiles.FirstOrDefault(p => p.Id == userId);
            if (profile == null || profile.Location == null)
            {
                return null;
            }
            var userLocationId = profile.Location;
            var location = _dbContext.Locations.FirstOrDefault(l => l.Id == userLocationId);
            var community = _dbContext.Communities.FirstOrDefault(c => c.Location == userLocationId);
            if (location == null || community == null)
            {
                return null;
            }
            return community;
        }
    }
}
