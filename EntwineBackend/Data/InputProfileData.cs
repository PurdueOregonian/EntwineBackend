using EntwineBackend.DbItems;

namespace Friends5___Backend.Data
{
    public class InputProfileData
    {
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public List<int>? Interests { get; set; }
        public InputLocation? Location { get; set; }
    }
}
