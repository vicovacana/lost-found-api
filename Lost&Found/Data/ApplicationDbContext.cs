using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;

namespace Lost_Found.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<StandardUser> StandardUsers => Set<StandardUser>();
        public DbSet<Admin> Admins => Set<Admin>();
        public DbSet<Listing> Listings => Set<Listing>();
        public DbSet<Claim> Claims => Set<Claim>();
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
