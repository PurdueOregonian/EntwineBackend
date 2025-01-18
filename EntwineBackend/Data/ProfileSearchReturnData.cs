using EntwineBackend.DbItems;

namespace Friends5___Backend.Data
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
