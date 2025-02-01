using EntwineBackend.DbItems;

namespace EntwineBackend.Data
{
    public class ProfileSearchReturnData
    {
        public string? Username { get; set; }
        public int? Age { get; set; }
        public Gender? Gender { get; set; }
        public List<int>? Interests { get; set; }
        public Location? Location { get; set; }
    }
}
