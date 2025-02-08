using EntwineBackend.DbContext;
using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public class CommunityChatService : ICommunityChatService
    {
        private readonly EntwineDbContext _dbContext;

        public CommunityChatService(EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<CommunityChatMessage> GetMessages(int chatId)
        {
            return _dbContext.CommunityChatMessages.Where(message => message.ChatId == chatId)
                .ToList();
        }

        public async Task<CommunityChatMessage> SendMessage(int chatId, int senderId, string content)
        {
            var newMessage = new CommunityChatMessage
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                TimeSent = DateTime.UtcNow
            };
            await _dbContext.CommunityChatMessages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();
            return newMessage;
        }

        public int? GetChatCommunity(int chatId)
        {
            return _dbContext.CommunityChats
                .Where(chat => chat.Id == chatId)
                .Select(chat => chat.Community)
                .FirstOrDefault();
        }
    }
}
