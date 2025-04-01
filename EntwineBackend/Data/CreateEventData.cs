namespace EntwineBackend.Data
{
    public class CreateEventData
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string? Title { get; set; }
        public int MaxParticipants { get; set; }
    }
}
