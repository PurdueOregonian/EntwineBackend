using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface ICommunityChatService
    {
        List<MessageReturnData> GetMessages(int chatId);
        Task<MessageReturnData> SendMessage(int chatId, int senderId, string content);
        int? GetChatCommunity(int chatId);
    }
}
