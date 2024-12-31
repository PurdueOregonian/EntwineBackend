using Friends5___Backend.DbItems;
using Npgsql;

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

        public async Task SaveProfile(string username, ReceivedProfileData data)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            await using (var cmd = dataSource.CreateCommand(@"
                INSERT INTO public.""Profiles""(""Username"", ""DateOfBirth"", ""Gender"")
                VALUES (@Username, @DateOfBirth, @Gender)
                ON CONFLICT (""Username"") 
                DO UPDATE 
                SET ""DateOfBirth"" = EXCLUDED.""DateOfBirth"", ""Gender"" = EXCLUDED.""Gender"";
            "))
            {
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
    }
}
