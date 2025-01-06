using Friends5___Backend.DbItems;

namespace Friends5___Backend.Services
{
    public interface IInterestService
    {
        public Task<List<Interest>> GetInterests();
        public Task<List<InterestCategory>> GetInterestCategories();
    }
}
