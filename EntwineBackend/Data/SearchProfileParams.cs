namespace EntwineBackend.Data
{
    public class SearchProfileParams
    {
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public List<Gender>? Gender { get; set; }
    }
}
