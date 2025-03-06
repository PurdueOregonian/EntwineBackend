using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface ICommunityChatService
    {
        List<CommunityChatMessageReturnData> GetMessages(int chatId);
        Task<CommunityChatMessageReturnData> SendMessage(int chatId, int senderId, string content);
        int? GetChatCommunity(int chatId);
    }
}
