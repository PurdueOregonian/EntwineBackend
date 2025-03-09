namespace EntwineBackend.DbItems
{
    public class MessageReturnData
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Content { get; set; }
        public DateTime? TimeSent { get; set; }
    }
}
