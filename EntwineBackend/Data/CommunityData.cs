namespace EntwineBackend.Data
{
    public class CommunityData
    {
        // Maybe send location ID later but for now we don't need it
        public string? State { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public List<int>? UserIds { get; set; }
        public List<CommunityChatData>? Chats { get; set; }
    }
}
