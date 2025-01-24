using EntwineBackend.DbItems;
using Friends5___Backend;
using Friends5___Backend.Data;
using Friends5___Backend.Services;
using Npgsql;
using NpgsqlTypes;

namespace EntwineBackend.Services
{
    public class ProfileService : IProfileService
    {
        private string _connectionString;

        public ProfileService(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<ProfileData?> GetProfile(int userId)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var sql = @"SELECT * FROM public.""Profiles""
                        WHERE ""Profiles"".""Id"" = @Id";

            using var command = dataSource.CreateCommand(sql);

            command.Parameters.AddWithValue("@Id", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var profile = new ProfileData
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    Gender = (Gender)reader.GetInt32(3),
                    Interests = reader.GetFieldValue<List<int>>(4),
                    Location = reader.IsDBNull(5) ? null : reader.GetInt32(5)
                };
                return profile;
            }
            else
            {
                return null;
            }
        }

        public async Task SaveProfile(int userId, string username, ServiceInputProfileData data)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            int? currentLocation = null;
            var selectSql = @"SELECT ""Location"" FROM public.""Profiles"" WHERE ""Id"" = @Id";
            using (var selectCmd = dataSource.CreateCommand(selectSql))
            {
                selectCmd.Parameters.AddWithValue("@Id", userId);
                using var reader = await selectCmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    currentLocation = reader.IsDBNull(0) ? null : reader.GetInt32(0);
                }
            }

            await using (var cmd = dataSource.CreateCommand(@"
                INSERT INTO public.""Profiles""(""Id"", ""Username"", ""DateOfBirth"", ""Gender"", ""Interests"", ""Location"")
                VALUES (@Id, @Username, @DateOfBirth, @Gender, @Interests, @Location)
                ON CONFLICT (""Id"") 
                DO UPDATE 
                SET ""DateOfBirth"" = EXCLUDED.""DateOfBirth"", ""Gender"" = EXCLUDED.""Gender"", ""Interests"" = EXCLUDED.""Interests"", ""Location"" = EXCLUDED.""Location"";
            "))
            {
                cmd.Parameters.AddWithValue("@Id", userId);
                cmd.Parameters.AddWithValue("@Username", username);
                if (data.DateOfBirth is not null)
                {
                    cmd.Parameters.AddWithValue("@DateOfBirth", data.DateOfBirth);
                }
                if (data.Gender is not null)
                {
                    cmd.Parameters.AddWithValue("@Gender", (int)data.Gender);
                }
                if (data.Interests is not null)
                {
                    cmd.Parameters.AddWithValue("@Interests", data.Interests);
                }
                if (data.Location is not null)
                {
                    cmd.Parameters.AddWithValue("@Location", data.Location);
                }
                await cmd.ExecuteNonQueryAsync();
            }

            if(data.Location is not null && data.Location != currentLocation)
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
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var sql = @"SELECT ""Username"" FROM public.""Profiles""
                        WHERE ""Id"" = @Id";

            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@Id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return reader.GetString(0);
            }
            else
            {
                return null;
            }
        }

        private async Task AddUserToCommunity(int userId, int locationId)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var selectSql = @"SELECT ""Id"", ""UserIds"" FROM public.""Communities"" WHERE ""Location"" = @Location";
            using var selectCmd = dataSource.CreateCommand(selectSql);
            selectCmd.Parameters.AddWithValue("@Location", locationId);
            using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var communityId = reader.GetInt32(0);
                var userIds = reader.IsDBNull(1) ? new List<int>() : reader.GetFieldValue<List<int>>(1);

                if (!userIds.Contains(userId))
                {
                    userIds.Add(userId);

                    var updateSql = @"UPDATE public.""Communities"" SET ""UserIds"" = @UserIds WHERE ""Id"" = @Id";
                    using var updateCmd = dataSource.CreateCommand(updateSql);
                    updateCmd.Parameters.AddWithValue("@UserIds", userIds);
                    updateCmd.Parameters.AddWithValue("@Id", communityId);
                    await updateCmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
