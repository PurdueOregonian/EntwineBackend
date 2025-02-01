using EntwineBackend.DbItems;
using EntwineBackend.Data;
using Npgsql;
using EntwineBackend.DbContext;

namespace EntwineBackend.Services
{
    public class CommunityService : ICommunityService
    {
        private string _connectionString;
        private readonly EntwineDbContext _entwineDbContext;

        public CommunityService(
            IConfiguration config,
            EntwineDbContext entwineDbContext)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            _entwineDbContext = entwineDbContext;
        }

        public async Task<CommunityData?> GetCommunity(int userId)
        {
            var profile = _entwineDbContext.Profiles.FirstOrDefault(p => p.Id == userId);
            if (profile == null)
            {
                return null;
            }
            var userLocationId = profile.Location;
            if (userLocationId == null)
            {
                return null;
            }
            var location = _entwineDbContext.Locations.FirstOrDefault(l => l.Id == userLocationId);

            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            var sql = @"SELECT * FROM public.""Communities"" WHERE ""Location"" = @userLocationId";
            await using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@userLocationId", userLocationId);
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }
            var community = new Community
            {
                Id = reader.GetInt32(0),
                Location = reader.GetInt32(1),
                UserIds = reader.GetFieldValue<List<int>>(2)
            };

            var communityChats = _entwineDbContext.CommunityChats
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
    }
}
