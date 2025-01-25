using EntwineBackend.DbItems;

namespace Friends5___Backend.Services
{
    public interface ILocationService
    {
        public Task<Location?> GetLocation(string latitude, string longitude);
        public Task<int> GetLocationId(InputLocation location);
        public Task<Location?> GetLocationById(int id);
    }
}
