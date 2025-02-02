using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface IInterestService
    {
        public List<Interest> GetInterests();
        public List<InterestCategory> GetInterestCategories();
    }
}
