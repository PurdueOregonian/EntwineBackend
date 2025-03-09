using EntwineBackend.DbContext;
using EntwineBackend.DbItems;
using Microsoft.EntityFrameworkCore;

namespace EntwineBackend.Services
{
    public class CommunityChatService : ICommunityChatService
    {
        private readonly EntwineDbContext _dbContext;

        public CommunityChatService(EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<MessageReturnData> GetMessages(int chatId)
        {
            return _dbContext.CommunityChatMessages
                .Include(m => m.Sender)
                .Where(message => message.ChatId == chatId)
                .Select(message => new MessageReturnData
                {
                    Id = message.Id,
                    Username = message.Sender.Username,
                    Content = message.Content,
                    TimeSent = message.TimeSent
                })
                .ToList();
        }

        public async Task<MessageReturnData> SendMessage(int chatId, int senderId, string content)
        {
            var user = _dbContext.Profiles.Find(senderId);
            var newMessage = new CommunityChatMessage
            {
                ChatId = chatId,
                Sender = user,
                Content = content,
                TimeSent = DateTime.UtcNow
            };
            await _dbContext.CommunityChatMessages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();
            return new MessageReturnData
            {
                Id = newMessage.Id,
                Username = user.Username,
                Content = content,
                TimeSent = newMessage.TimeSent
            };
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
