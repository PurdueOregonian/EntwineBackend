using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface IChatService
    {
        public List<Chat> GetChats(int userId);
        public List<Message> GetMessages(int chatId);
        public Chat? GetChat(int chatId);
        public Task<Chat?> CreateChat(List<int> userIds);
        public Task<Message?> SendMessage(int chatId, int senderId, string content);
    }
}
