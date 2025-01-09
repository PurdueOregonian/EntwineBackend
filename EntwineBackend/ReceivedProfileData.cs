namespace EntwineBackend
{
    public class ReceivedProfileData
    {
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public List<int>? Interests { get; set; }
    }
}
