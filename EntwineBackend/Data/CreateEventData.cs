namespace EntwineBackend.Data
{
    public class CreateEventData
    {
        public int CommunityId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Name { get; set; }
        public int MaxParticipants { get; set; }
    }
}
