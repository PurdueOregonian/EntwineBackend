using EntwineBackend.DbItems;

namespace Friends5___Backend
{
    public static class Constants
    {
        public static readonly InterestCategory[] InterestCategories = [
            new InterestCategory { Id = 1, Name = "Creativity" },
                new InterestCategory { Id = 2, Name = "Nature" },
                new InterestCategory { Id = 3, Name = "Technology" },
                new InterestCategory { Id = 4, Name = "Fitness & Sports" },
                new InterestCategory { Id = 5, Name = "Travel" },
                new InterestCategory { Id = 6, Name = "Music" }
            ];

        public static readonly Interest[] Interests = [
            new Interest { Id = 1, Categories = [1], Name = "Digital Art" },
                new Interest { Id = 2, Categories = [1], Name = "Photography" },
                new Interest { Id = 3, Categories = [1], Name = "Painting" },
                new Interest { Id = 4, Categories = [1], Name = "Knitting" },
                new Interest { Id = 5, Categories = [1, 6], Name = "Music Composition" },
                new Interest { Id = 6, Categories = [1, 6], Name = "Cooking" },
                new Interest { Id = 7, Categories = [1, 6], Name = "Woodworking" },
                new Interest { Id = 8, Categories = [2, 4], Name = "Hiking" },
                new Interest { Id = 9, Categories = [2], Name = "Gardening" },
                new Interest { Id = 10, Categories = [1, 3], Name = "Programming" },
                new Interest { Id = 11, Categories = [1, 3], Name = "Hardware Assembly" },
                new Interest { Id = 12, Categories = [4], Name = "Basketball" },
                new Interest { Id = 13, Categories = [4], Name = "Soccer" },
                new Interest { Id = 14, Categories = [5], Name = "Backpacking" },
                new Interest { Id = 15, Categories = [6], Name = "Jazz" },
                new Interest { Id = 16, Categories = [6], Name = "Pop" }
            ];
    }
}
