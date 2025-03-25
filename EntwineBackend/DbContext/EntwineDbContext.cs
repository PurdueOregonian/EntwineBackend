using EntwineBackend.DbItems;
using EntwineBackend.UserId;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EntwineBackend.DbContext
{
    public class EntwineDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public EntwineDbContext(DbContextOptions options) : base(options)
        {
            Profiles = Set<ProfileData>();
            Chats = Set<Chat>();
            CommunityChats = Set<CommunityChat>();
            Messages = Set<Message>();
            CommunityChatMessages = Set<CommunityChatMessage>();
            Communities = Set<Community>();
            InterestCategories = Set<InterestCategory>();
            Interests = Set<Interest>();
            Locations = Set<Location>();
            ApplicationUserLogins = Set<ApplicationUserLogin>();
            Events = Set<Event>();
        }

        public DbSet<ProfileData> Profiles { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<CommunityChat> CommunityChats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<CommunityChatMessage> CommunityChatMessages { get; set; }
        public DbSet<Community> Communities { get; set; }
        public DbSet<InterestCategory> InterestCategories { get; set; }
        public DbSet<Interest> Interests { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ApplicationUserLogin> ApplicationUserLogins { get; set; }
        public DbSet<Event> Events { get; set; }

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

            modelBuilder.Entity<InterestCategory>().HasData(Constants.InterestCategories);

            modelBuilder.Entity<Interest>().HasData(Constants.Interests);
        }
    }
}
