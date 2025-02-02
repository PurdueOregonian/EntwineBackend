using EntwineBackend.DbItems;
using EntwineBackend;
using EntwineBackend.Data;
using EntwineBackend.Services;
using Npgsql;
using NpgsqlTypes;
using EntwineBackend.DbContext;

namespace EntwineBackend.Services
{
    public class ProfileService : IProfileService
    {
        private string _connectionString;
        private readonly EntwineDbContext _dbContext;

        public ProfileService(IConfiguration config, EntwineDbContext dbContext)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
            _dbContext = dbContext;
        }

        public async Task<ProfileData?> GetProfile(int userId)
        {
            return _dbContext.Profiles.FirstOrDefault(p => p.Id == userId);
        }

        public async Task SaveProfile(int userId, string username, ServiceInputProfileData data)
        {
            var currentProfile = _dbContext.Profiles.FirstOrDefault(p => p.Id == userId);
            var currentLocation = currentProfile?.Location;

            if (currentProfile != null)
            {
                currentProfile.Username = username;
                currentProfile.DateOfBirth = data.DateOfBirth;
                currentProfile.Gender = data.Gender;
                currentProfile.Interests = data.Interests;
                currentProfile.Location = data.Location;
                _dbContext.Profiles.Update(currentProfile);
            }
            else
            {
                var newProfile = new ProfileData
                {
                    Username = username,
                    DateOfBirth = data.DateOfBirth,
                    Gender = data.Gender,
                    Interests = data.Interests,
                    Location = data.Location
                };
                _dbContext.Profiles.Add(newProfile);
            }
            await _dbContext.SaveChangesAsync();

            if (data.Location is not null && data.Location != currentLocation)
            {
                await AddUserToCommunity(userId, data.Location.Value);
            }
        }

        public async Task<List<ProfileData>> SearchUsers(int userId, string searchString)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var users = new List<ProfileData>();

            var sql = @"SELECT * FROM public.""AspNetUsers"" WHERE ""UserName"" ILIKE @SearchString";

            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@SearchString", $"%{searchString}%");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var user = new ProfileData
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1)
                };
                if (user.Id != userId)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<List<ProfileData>> SearchProfiles(int userId, SearchProfileParams data)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var profiles = new List<ProfileData>();

            var query = new List<string>
            {
                @"SELECT * FROM public.""Profiles"" WHERE",
                @"DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) >= @MinAge",
                @"AND DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) <= @MaxAge"
            };

            if (data.Gender != null && data.Gender.Count > 0)
            {
                query.Add(@"AND ""Profiles"".""Gender"" = ANY(@Gender)");
            }

            var sql = string.Join(" ", query);

            using var command = dataSource.CreateCommand(sql);

            command.Parameters.AddWithValue("@MinAge", data.MinAge);
            command.Parameters.AddWithValue("@MaxAge", data.MaxAge);

            if (data.Gender != null && data.Gender.Count > 0)
            {
                command.Parameters.AddWithValue("@Gender", NpgsqlDbType.Array | NpgsqlDbType.Integer, data.Gender.Select(g => (int)g).ToArray());
            }

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var profile = new ProfileData
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    Gender = (Gender)reader.GetInt32(3),
                    Interests = reader.GetFieldValue<List<int>>(4)
                };
                if (profile.Id != userId)
                {
                    profiles.Add(profile);
                }
            }
            return profiles;
        }

        public async Task<string?> GetUsernameFromId(int id)
        {
            return _dbContext.Profiles.FirstOrDefault(p => p.Id == id)?.Username;
        }

        private async Task AddUserToCommunity(int userId, int locationId)
        {
            var community = _dbContext.Communities.FirstOrDefault(c => c.Location == locationId);
            if(community != null && community.UserIds != null && !community.UserIds.Contains(userId))
            {
                var newUserIds = community.UserIds.Append(userId).ToList();
                community.UserIds = newUserIds;
                _dbContext.Communities.Update(community);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
