using EntwineBackend.DbItems;
using Npgsql;

namespace Friends5___Backend.Services
{
    public class CommunityService : ICommunityService
    {
        private string _connectionString;

        public CommunityService(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<Community?> GetCommunity(int userId)
        {
            // Get the community the user is a part of. Subject to change
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            var userLocationSql = @"SELECT ""Location"" FROM public.""Profiles"" WHERE ""Id"" = @userId";
            await using var userLocationCommand = dataSource.CreateCommand(userLocationSql);
            userLocationCommand.Parameters.AddWithValue("@userId", userId);
            var userLocation = await userLocationCommand.ExecuteScalarAsync();
            if (userLocation == null)
            {
                return null;
            }

            var sql = @"SELECT * FROM public.""Communities"" WHERE ""Location"" = @userLocation";
            await using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@userLocation", userLocation);
            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var community = new Community
                {
                    Id = reader.GetInt32(0),
                    Location = reader.GetInt32(1),
                    UserIds = reader.GetFieldValue<List<int>>(2)
                };
                return community;
            }
            else
            {
                return null;
            }
        }
    }
}
