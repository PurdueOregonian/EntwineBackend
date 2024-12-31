using Friends5___Backend.DbItems;
using Friends5___Backend.UserId;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Friends5___Backend.DbContext
{
    public class FriendsDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public FriendsDbContext(DbContextOptions options) : base(options) { }

        public DbSet<ProfileData> Profiles { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ApplicationUserLogin> ApplicationUserLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
                entity.Property(l => l.Id)
                .HasColumnType("int")
                .IsRequired()
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();

                entity.HasKey(l => l.Id);
            });
        }
    }
}
