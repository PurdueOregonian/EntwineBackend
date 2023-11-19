using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Friends5___Backend.DbContext
{
    public class FriendsDbContext : IdentityDbContext
    {
        public FriendsDbContext(DbContextOptions options): base(options) { }

        public DbSet<ProfileData> Profiles { get; set; }
    }
}
