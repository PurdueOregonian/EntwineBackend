using Friends5___Backend.DbItems;
using Npgsql;
using NpgsqlTypes;

namespace Friends5___Backend.Services
{
    public class ProfileService : IProfileService
    {
        private string _connectionString;

        public ProfileService(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<ProfileData?> GetProfile(string username)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var sql = @"SELECT * FROM public.""Profiles""
                        WHERE ""Profiles"".""Username"" = @Username";

            using var command = dataSource.CreateCommand(sql);

            command.Parameters.AddWithValue("@Username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var profile = new ProfileData
                {
                    Id = reader.GetInt32(0),
                    Username = reader.GetString(1),
                    DateOfBirth = DateOnly.FromDateTime(reader.GetDateTime(2)),
                    Gender = (Gender)reader.GetInt32(3)
                };
                return profile;
            }
            else
            {
                return null;
            }
        }

        public async Task SaveProfile(int userId, string username, ReceivedProfileData data)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using (var cmd = dataSource.CreateCommand(@"
                INSERT INTO public.""Profiles""(""Id"", ""Username"", ""DateOfBirth"", ""Gender"")
                VALUES (@Id, @Username, @DateOfBirth, @Gender)
                ON CONFLICT (""Id"") 
                DO UPDATE 
                SET ""DateOfBirth"" = EXCLUDED.""DateOfBirth"", ""Gender"" = EXCLUDED.""Gender"";
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
                await cmd.ExecuteNonQueryAsync();
            }
        }
        public async Task<List<ProfileData>> SearchUsers(string username, string searchString)
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
                if (user.Username != username)
                {
                    users.Add(user);
                }
            }

            return users;
        }

        public async Task<List<ProfileData>> SearchProfiles(string username, SearchProfileData data)
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
                    Gender = (Gender)reader.GetInt32(3)
                };
                if (profile.Username != username)
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
    }
}
