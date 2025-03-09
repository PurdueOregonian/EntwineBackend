using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface ILocationService
    {
        public Task<Location?> GetLocation(string latitude, string longitude);
    }
}
