using EntwineBackend.DbItems;
using EntwineBackend.Data;
using Npgsql;
using NpgsqlTypes;
using EntwineBackend.DbContext;
using Microsoft.EntityFrameworkCore;

namespace EntwineBackend.Services
{
    public class ProfileService : IProfileService
    {
        private readonly EntwineDbContext _dbContext;

        public ProfileService(EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ProfileData? GetProfile(int userId)
        {
            return _dbContext.Profiles.Include(p => p.Location).FirstOrDefault(p => p.Id == userId);
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

            if (data.Location is not null && data.Location.Id != currentLocation?.Id)
            {
                await AddUserToCommunity(userId, data.Location.Id);
            }
        }

        public List<UserSearchResult> SearchUsers(int userId, string searchString)
        {
            var matchingUsers = _dbContext.Users.Where(user => user.UserName!.Contains(searchString) && user.Id != userId);
            return matchingUsers.Select(user => new UserSearchResult { Id = user.Id, Username = user.UserName }).ToList();
        }

        public List<ProfileData> SearchProfiles(int userId, SearchProfileParams data)
        {
            var sqlCommand = @"SELECT * FROM public.""Profiles"" WHERE " +
                     @"DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) >= @MinAge " +
                     @"AND DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) <= @MaxAge ";
            if (data.Gender != null && data.Gender.Count > 0)
            {
                sqlCommand += @"AND ""Profiles"".""Gender"" = ANY(@Gender)";
            }

            var minAgeParam = new NpgsqlParameter("MinAge", data.MinAge);
            var maxAgeParam = new NpgsqlParameter("MaxAge", data.MaxAge);
            var gendersParam = new NpgsqlParameter("Gender", NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = data.Gender?.Select(g => (int)g).ToArray()
            };

            var result = _dbContext.Profiles
                .FromSqlRaw(sqlCommand, minAgeParam, maxAgeParam, gendersParam)
                .ToList();

            return result;
        }

        public string? GetUsernameFromId(int id)
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
