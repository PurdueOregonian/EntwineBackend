using EntwineBackend.DbContext;
using EntwineBackend.DbItems;
using Npgsql;

namespace EntwineBackend.Services
{
    public class InterestService : IInterestService
    {
        private readonly EntwineDbContext _dbContext;

        public InterestService(EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<InterestCategory> GetInterestCategories()
        {
            return [.. _dbContext.InterestCategories];
        }

        public List<Interest> GetInterests()
        {
            return [.. _dbContext.Interests];
        }
    }
}
