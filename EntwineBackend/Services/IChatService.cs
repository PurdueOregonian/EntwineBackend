using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface IChatService
    {
        public List<Chat> GetChats(int userId);
        public List<MessageReturnData> GetMessages(int chatId);
        public Chat? GetChat(int chatId);
        public Task<Chat?> CreateChat(List<int> userIds);
        public Task<MessageReturnData> SendMessage(int chatId, int senderId, string content);
    }
}
