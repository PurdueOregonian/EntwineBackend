namespace EntwineBackend
{
    public class SearchProfileData
    {
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public List<Gender>? Gender { get; set; }
    }
}
