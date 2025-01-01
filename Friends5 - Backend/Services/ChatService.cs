using Friends5___Backend.DbItems;
using Npgsql;

namespace Friends5___Backend.Services
{
    public class ChatService : IChatService
    {
        private string _connectionString;

        public ChatService(IConfiguration config)
        {
            _connectionString = config.GetValue<string>("ConnectionStrings:DefaultConnection")!;
        }

        public async Task<List<Chat>> GetChats(int userId)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var chats = new List<Chat>();

            var sql = @"SELECT * FROM public.""Chats""
                        WHERE ""UserIds"" @> ARRAY[@UserId]";

            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var chat = new Chat
                {
                    Id = reader.GetInt32(0),
                    UserIds = reader.GetFieldValue<List<int>>(1)
                };
                chats.Add(chat);
            }

            return chats;
        }

        public async Task<List<Message>> GetMessages(int chatId)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var messages = new List<Message>();

            var sql = @"SELECT * FROM public.""Messages""
                        WHERE ""ChatId"" = @ChatId";

            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@ChatId", chatId);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var message = new Message
                {
                    Id = reader.GetInt32(0),
                    ChatId = reader.GetInt32(1),
                    SenderId = reader.GetInt32(2),
                    Content = reader.GetString(3),
                    TimeSent = reader.GetDateTime(4)
                };
                messages.Add(message);
            }

            return messages;
        }

        public async Task<Chat?> GetChat(int chatId)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);

            var sql = @"SELECT * FROM public.""Chats""
                        WHERE ""Id"" = @ChatId";

            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@ChatId", chatId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var chat = new Chat
                {
                    Id = reader.GetInt32(0),
                    UserIds = reader.GetFieldValue<List<int>>(1)
                };
                return chat;
            }

            return null;
        }

        public async Task<Chat?> CreateChat(List<int> userIds)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            var sql = @"INSERT INTO public.""Chats"" (""UserIds"")
                        VALUES (@UserIds)
                        RETURNING ""Id""";
            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@UserIds", userIds);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var chatId = reader.GetInt32(0);
                return new Chat
                {
                    Id = chatId,
                    UserIds = userIds
                };
            }
            return null;
        }

        public async Task<Message?> SendMessage(MessageToSend message)
        {
            await using var dataSource = NpgsqlDataSource.Create(_connectionString);
            var sql = @"INSERT INTO public.""Messages"" (""ChatId"", ""SenderId"", ""Content"", ""TimeSent"")
                        VALUES (@ChatId, @SenderId, @Content, @TimeSent)
                        RETURNING ""Id"", ""ChatId"", ""SenderId"", ""Content"", ""TimeSent""";
            using var command = dataSource.CreateCommand(sql);
            command.Parameters.AddWithValue("@ChatId", message.ChatId);
            command.Parameters.AddWithValue("@SenderId", message.SenderId);
            command.Parameters.AddWithValue("@Content", message.Content);
            command.Parameters.AddWithValue("@TimeSent", DateTime.Now);
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var messageRead = new Message
                {
                    Id = reader.GetInt32(0),
                    ChatId = reader.GetInt32(1),
                    SenderId = reader.GetInt32(2),
                    Content = reader.GetString(3),
                    TimeSent = reader.GetDateTime(4)
                };
                return messageRead;
            }
            return null;
        }
    }
}
