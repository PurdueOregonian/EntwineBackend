using EntwineBackend;
using EntwineBackend.Data;
using EntwineBackend.DbContext;
using EntwineBackend.DbItems;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;

namespace Friends5___Backend
{
    public static class DbFunctions
    {
        public static List<Chat> GetChats(EntwineDbContext dbContext, int userId)
        {
            var chats = dbContext.Chats
                .Where(chat => chat.UserIds != null && chat.UserIds.Contains(userId))
                .ToList();

            return chats;
        }

        public static List<MessageReturnData> GetMessages(EntwineDbContext dbContext, int chatId)
        {
            return [.. dbContext.Messages
                .Include(m => m.Sender)
                .Where(message => message.ChatId == chatId)
                .Select(message => new MessageReturnData
                {
                    Id = message.Id,
                    Username = message.Sender.Username,
                    UserId = message.Sender.Id,
                    Content = message.Content,
                    TimeSent = message.TimeSent
                })];
        }

        public static Chat? GetChat(EntwineDbContext dbContext, int chatId)
        {
            return dbContext.Chats.FirstOrDefault(chat => chat.Id == chatId);
        }

        public static async Task<Chat?> CreateChat(EntwineDbContext dbContext, List<int> userIds)
        {
            if (userIds.Count < 2)
            {
                return null;
            }
            // Ok for this to be kinda expensive since it is uncommon to create a chat
            // TODO SequenceEqual doesn't ignore order, fix this
            var existingChat = dbContext.Chats
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
            dbContext.Chats.Add(newChat);
            await dbContext.SaveChangesAsync();

            return newChat;
        }

        public static async Task<MessageReturnData> SendMessage(
            EntwineDbContext dbContext,
            int chatId,
            int senderId,
            string content)
        {
            var sender = dbContext.Profiles.Find(senderId);
            var newMessage = new Message
            {
                ChatId = chatId,
                Sender = sender,
                Content = content,
                TimeSent = DateTime.UtcNow
            };
            await dbContext.Messages.AddAsync(newMessage);
            await dbContext.SaveChangesAsync();
            return new MessageReturnData
            {
                Id = newMessage.Id,
                Username = sender.Username,
                UserId = sender.Id,
                Content = content,
                TimeSent = newMessage.TimeSent
            };
        }

        public static List<MessageReturnData> CommunityGetMessages(
            EntwineDbContext dbContext,
            int chatId)
        {
            return [.. dbContext.CommunityChatMessages
                .Include(m => m.Sender)
                .Where(message => message.ChatId == chatId)
                .Select(message => new MessageReturnData
                {
                    Id = message.Id,
                    Username = message.Sender.Username,
                    UserId = message.Sender.Id,
                    Content = message.Content,
                    TimeSent = message.TimeSent
                })];
        }

        public static async Task<MessageReturnData> CommunitySendMessage(
            EntwineDbContext dbContext,
            int chatId,
            int senderId,
            string content)
        {
            var user = dbContext.Profiles.Find(senderId);
            var newMessage = new CommunityChatMessage
            {
                ChatId = chatId,
                Sender = user,
                Content = content,
                TimeSent = DateTime.UtcNow
            };
            await dbContext.CommunityChatMessages.AddAsync(newMessage);
            await dbContext.SaveChangesAsync();
            return new MessageReturnData
            {
                Id = newMessage.Id,
                Username = user.Username,
                UserId = user.Id,
                Content = content,
                TimeSent = newMessage.TimeSent
            };
        }

        public static int? GetChatCommunity(EntwineDbContext dbContext, int chatId)
        {
            return dbContext.CommunityChats
                .Where(chat => chat.Id == chatId)
                .Select(chat => chat.Community)
                .FirstOrDefault();
        }

        public static CommunityData? GetCommunityData(EntwineDbContext dbContext, int userId)
        {
            var profile = dbContext.Profiles.Include(p => p.Location).FirstOrDefault(p => p.Id == userId);
            if (profile?.Location == null)
            {
                return null;
            }
            var userLocationId = profile.Location.Id;
            var location = profile.Location;

            var community = dbContext.Communities.FirstOrDefault(c => c.Location == userLocationId);

            if (location == null || community == null)
            {
                return null;
            }

            var communityChats = dbContext.CommunityChats
                .Where(chat => chat.Community == community.Id)
                .Select(chat => new CommunityChatData
                {
                    Id = chat.Id,
                    Name = chat.Name
                })
                .ToList();

            return new CommunityData
            {
                State = location.State,
                City = location.City,
                Country = location.Country,
                UserIds = community.UserIds,
                Chats = communityChats
            };
        }

        public static Community? GetCommunity(EntwineDbContext dbContext, int userId)
        {
            var profile = dbContext.Profiles.Include(p => p.Location).FirstOrDefault(p => p.Id == userId);
            if (profile == null || profile.Location == null)
            {
                return null;
            }
            var userLocationId = profile.Location.Id;
            var location = profile.Location;
            var community = dbContext.Communities.FirstOrDefault(c => c.Location == userLocationId);
            if (location == null || community == null)
            {
                return null;
            }
            return community;
        }

        public static List<InterestCategory> GetInterestCategories(EntwineDbContext dbContext)
        {
            return [.. dbContext.InterestCategories];
        }

        public static List<Interest> GetInterests(EntwineDbContext dbContext)
        {
            return [.. dbContext.Interests];
        }

        public static async Task<Location> GetLocationWithId(EntwineDbContext dbContext, InputLocation location)
        {
            var matchingLocation = dbContext.Locations.FirstOrDefault(
                l => l.City == location.City && l.Country == location.Country && l.State == location.State);
            if (matchingLocation != null)
            {
                return matchingLocation;
            }
            else
            {
                // Create new location and return Id
                // We also need to add a Community
                var newLocation = new Location
                {
                    City = location.City,
                    Country = location.Country,
                    State = location.State
                };
                dbContext.Locations.Add(newLocation);
                await dbContext.SaveChangesAsync();

                // The newLocation.Id will be populated with the generated ID
                await CreateNewCommunity(dbContext, newLocation.Id);

                return newLocation;
            }
        }

        public static Location? GetLocationById(EntwineDbContext dbContext, int locationId)
        {
            return dbContext.Locations.FirstOrDefault(l => l.Id == locationId);
        }

        public static ProfileData? GetProfile(EntwineDbContext dbContext, int userId)
        {
            return dbContext.Profiles.Include(p => p.Location).FirstOrDefault(p => p.Id == userId);
        }

        public static async Task SaveProfile(
            EntwineDbContext dbContext,
            int userId,
            string username,
            ServiceInputProfileData data)
        {
            var currentProfile = dbContext.Profiles.FirstOrDefault(p => p.Id == userId);
            var currentLocation = currentProfile?.Location;

            if (currentProfile != null)
            {
                currentProfile.Username = username;
                currentProfile.DateOfBirth = data.DateOfBirth;
                currentProfile.Gender = data.Gender;
                currentProfile.Interests = data.Interests;
                currentProfile.Location = data.Location;
                dbContext.Profiles.Update(currentProfile);
            }
            else
            {
                var newProfile = new ProfileData
                {
                    Username = username,
                    DateOfBirth = data.DateOfBirth,
                    Gender = data.Gender,
                    Interests = data.Interests,
                    Location = data.Location
                };
                dbContext.Profiles.Add(newProfile);
            }
            await dbContext.SaveChangesAsync();

            if (data.Location is not null && data.Location.Id != currentLocation?.Id)
            {
                await AddUserToCommunity(dbContext, userId, data.Location.Id);
            }
        }

        public static List<UserSearchResult> SearchUsers(
            EntwineDbContext dbContext,
            int userId,
            string searchString)
        {
            var matchingUsers = dbContext.Users.Where(user => user.UserName!.Contains(searchString) && user.Id != userId);
            return [.. matchingUsers.Select(user => new UserSearchResult { Id = user.Id, Username = user.UserName })];
        }

        public static List<ProfileData> SearchProfiles(
            EntwineDbContext dbContext,
            SearchProfileParams data)
        {
            var sqlCommand = @"SELECT * FROM public.""Profiles"" WHERE " +
                     @"DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) >= @MinAge " +
                     @"AND DATE_PART('year', AGE(""Profiles"".""DateOfBirth"")) <= @MaxAge ";
            if (data.Gender != null && data.Gender.Count > 0)
            {
                sqlCommand += @"AND ""Profiles"".""Gender"" = ANY(@Gender)";
            }

            var minAgeParam = new NpgsqlParameter("MinAge", data.MinAge);
            var maxAgeParam = new NpgsqlParameter("MaxAge", data.MaxAge);
            var gendersParam = new NpgsqlParameter("Gender", NpgsqlDbType.Array | NpgsqlDbType.Integer)
            {
                Value = data.Gender?.Select(g => (int)g).ToArray()
            };

            var result = dbContext.Profiles
                .FromSqlRaw(sqlCommand, minAgeParam, maxAgeParam, gendersParam)
                .ToList();

            return result;
        }

        public static string? GetUsernameFromId(EntwineDbContext dbContext, int id)
        {
            return dbContext.Profiles.FirstOrDefault(p => p.Id == id)?.Username;
        }

        public static async Task<Event> CreateEvent(EntwineDbContext dbContext, int userId, CreateEventData data)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var community = dbContext.Communities.FirstOrDefault(c => c.Id == data.CommunityId);
            var newEvent = new Event
            {
                Community = data.CommunityId,
                Start = data.StartTime,
                End = data.EndTime,
                OrganizerId = userId,
                Title = data.Name,
                UserIds = [userId],
                MaxParticipants = data.MaxParticipants
            };
            dbContext.Events.Add(newEvent);
            await dbContext.SaveChangesAsync();
            return newEvent;
        }

        public static List<Event> GetEvents(EntwineDbContext dbContext, int userId)
        {
            var community = GetCommunity(dbContext, userId);
            if (community == null)
            {
                return [];
            }
            return [.. dbContext.Events
                .Where(e => e.Community == community.Id && e.Start > DateTime.UtcNow)
                .OrderBy(e => e.Start)];
        }

        private static async Task AddUserToCommunity(
            EntwineDbContext dbContext,
            int userId,
            int locationId)
        {
            var community = dbContext.Communities.FirstOrDefault(c => c.Location == locationId);
            if (community != null && community.UserIds != null && !community.UserIds.Contains(userId))
            {
                var newUserIds = community.UserIds.Append(userId).ToList();
                community.UserIds = newUserIds;
                dbContext.Communities.Update(community);
                await dbContext.SaveChangesAsync();
            }
        }

        private static async Task CreateNewCommunity(EntwineDbContext dbContext, int locationId)
        {
            var community = new Community
            {
                Location = locationId,
                UserIds = []
            };
            dbContext.Communities.Add(community);
            await dbContext.SaveChangesAsync();

            // Now we need to create the default chats for the community. For each interest category, create a chat
            var defaultChats = Constants.InterestCategories.Select(category =>
                new CommunityChat
                {
                    Name = category.Name,
                    Community = community.Id
                }
            ).ToList();
            dbContext.CommunityChats.AddRange(defaultChats);
            await dbContext.SaveChangesAsync();
        }
    }
}
