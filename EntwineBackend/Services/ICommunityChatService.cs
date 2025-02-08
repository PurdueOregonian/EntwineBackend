using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public interface ICommunityChatService
    {
        List<CommunityChatMessage> GetMessages(int chatId);
        Task<CommunityChatMessage> SendMessage(int chatId, int senderId, string content);
        int? GetChatCommunity(int chatId);
    }
}
