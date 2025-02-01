namespace EntwineBackend.Data
{
    public class ServiceInputProfileData
    {
        public DateOnly? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public List<int>? Interests { get; set; }
        public int? Location { get; set; }
    }
}
