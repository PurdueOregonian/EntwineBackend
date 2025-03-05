using EntwineBackend.DbContext;
using Microsoft.EntityFrameworkCore;

namespace EntwineBackend
{
    public class Utils
    {
        public static int YearsSince(DateOnly date)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            int yearsDifference = currentDate.Year - date.Year;

            return currentDate < date.AddYears(yearsDifference) ? yearsDifference - 1 : yearsDifference;
        }

        public static bool IsUserInChatCommunity(EntwineDbContext dbContext, int userId, int chatId)
        {
            //TODO change when user has community list
            //TODO probably not efficient to include location all the time
            var userLocation = dbContext.Profiles.Include(p => p.Location).FirstOrDefault(p => p.Id == userId)?.Location;
            if(userLocation is null)
            {
                return false;
            }
            var userCommunity = dbContext.Communities.FirstOrDefault(c => c.Location == userLocation.Id);
            var community = dbContext.CommunityChats
                .Where(chat => chat.Id == chatId)
                .Select(chat => chat.Community)
                .FirstOrDefault();

            return userCommunity?.Id == community;
        }
    }
}
