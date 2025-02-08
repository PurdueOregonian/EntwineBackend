using EntwineBackend.DbContext;
using EntwineBackend.DbItems;

namespace EntwineBackend.Services
{
    public class ChatService : IChatService
    {
        private readonly EntwineDbContext _dbContext;

        public ChatService(EntwineDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<Chat> GetChats(int userId)
        {
            var chats = _dbContext.Chats
                .Where(chat => chat.UserIds != null && chat.UserIds.Contains(userId))
                .ToList();
            
            return chats;
        }

        public List<Message> GetMessages(int chatId)
        {
            return _dbContext.Messages.Where(message => message.ChatId == chatId)
                .ToList();
        }

        public Chat? GetChat(int chatId)
        {
            return _dbContext.Chats.FirstOrDefault(chat => chat.Id == chatId);
        }

        public async Task<Chat?> CreateChat(List<int> userIds)
        {
            if(userIds.Count < 2)
            {
                return null;
            }
            // Ok for this to be kinda expensive since it is uncommon to create a chat
            // TODO SequenceEqual doesn't ignore order, fix this
            var existingChat = _dbContext.Chats
                .Where(chat => chat.UserIds != null && chat.UserIds.SequenceEqual(userIds))
                .FirstOrDefault();
            if (existingChat != null)
            {
                return null;
            }

            var newChat = new Chat
            {
                UserIds = userIds
            };
            _dbContext.Chats.Add(newChat);
            await _dbContext.SaveChangesAsync();

            return newChat;
        }

        public async Task<Message> SendMessage(int chatId, int senderId, string content)
        {
            var newMessage = new Message
            {
                ChatId = chatId,
                SenderId = senderId,
                Content = content,
                TimeSent = DateTime.UtcNow
            };
            await _dbContext.Messages.AddAsync(newMessage);
            await _dbContext.SaveChangesAsync();
            return newMessage;
        }
    }
}
