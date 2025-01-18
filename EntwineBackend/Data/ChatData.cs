namespace Friends5___Backend.Data
{
    public class ChatData
    {
        public int Id { get; set; }
        public List<string>? Usernames { get; set; } // All users in the chat EXCLUDING the requesting user
    }
}
