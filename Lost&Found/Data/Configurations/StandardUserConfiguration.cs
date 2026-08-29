using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class StandardUserConfiguration : IEntityTypeConfiguration<StandardUser>
    {
        public void Configure(EntityTypeBuilder<StandardUser> builder)
        {
            builder.HasMany(s => s.Listings)
                .WithOne(o => o.Creator)
                .HasForeignKey(o => o.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Claims)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
