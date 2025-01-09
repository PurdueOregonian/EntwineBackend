using EntwineBackend.DbItems;
using EntwineBackend.Migrations;
using Npgsql;

namespace EntwineBackend.Services
{
    public class InterestService : IInterestService
    {
        private string _connectionString;

        public InterestService(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<List<InterestCategory>> GetInterestCategories()
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var sql = @"SELECT * FROM public.""InterestCategories""";
            using var reader = await dataSource.CreateCommand(sql).ExecuteReaderAsync();

            var interestCategories = new List<InterestCategory>();
            while (await reader.ReadAsync())
            {
                var interestCategory = new InterestCategory
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
                interestCategories.Add(interestCategory);
            }

            return interestCategories;
        }

        public async Task<List<Interest>> GetInterests()
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var sql = @"SELECT * FROM public.""Interests""";
            using var reader = await dataSource.CreateCommand(sql).ExecuteReaderAsync();

            var interests = new List<Interest>();
            while (await reader.ReadAsync())
            {
                var interest = new Interest
                {
                    Id = reader.GetInt32(0),
                    Categories = reader.GetFieldValue<List<int>>(1),
                    Name = reader.GetString(2)
                };
                interests.Add(interest);
            }

            return interests;
        }
    }
}
