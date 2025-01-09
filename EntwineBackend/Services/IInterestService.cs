using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface IInterestService
    {
        public Task<List<Interest>> GetInterests();
        public Task<List<InterestCategory>> GetInterestCategories();
    }
}
