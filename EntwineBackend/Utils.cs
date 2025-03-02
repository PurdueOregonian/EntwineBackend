﻿using EntwineBackend.DbContext;

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
            var userLocation = dbContext.Profiles.FirstOrDefault(p => p.Id == userId)?.Location;
            var userCommunity = dbContext.Communities.FirstOrDefault(c => c.Location == userLocation);
            var community = dbContext.CommunityChats
                .Where(chat => chat.Id == chatId)
                .Select(chat => chat.Community)
                .FirstOrDefault();

            return userCommunity?.Id == community;
        }
    }
}
