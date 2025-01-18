using Friends5___Backend;

namespace EntwineBackend.DbItems
{
    public class OwnProfileReturnData
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public List<int>? Interests { get; set; }
        public Location? Location { get; set; }
    }
}
