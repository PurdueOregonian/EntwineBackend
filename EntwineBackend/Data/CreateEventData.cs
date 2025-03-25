namespace EntwineBackend.Data
{
    public class CreateEventData
    {
        public int CommunityId { get; set; }
        public DateTime Time { get; set; }
        public string? Name { get; set; }
        public int MaxParticipants { get; set; }
    }
}
