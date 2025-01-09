using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface IChatService
    {
        public Task<List<Chat>> GetChats(int userId);
        public Task<List<Message>> GetMessages(int chatId);
        public Task<Chat?> GetChat(int chatId);
        public Task<Chat?> CreateChat(List<int> userIds);
        public Task<Message?> SendMessage(int chatId, int senderId, string content);
    }
}
