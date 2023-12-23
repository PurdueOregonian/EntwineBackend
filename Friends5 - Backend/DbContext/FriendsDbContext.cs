using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Friends5___Backend.DbContext
{
    public class FriendsDbContext : IdentityDbContext<IdentityUser>
    {
        public FriendsDbContext(DbContextOptions options): base(options) { }

        public DbSet<ProfileData> Profiles { get; set; }
    }
}
