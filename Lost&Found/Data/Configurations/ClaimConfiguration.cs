using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class ClaimConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.ToTable("Potrazivanje", t =>
                t.HasCheckConstraint("CK_Potrazivanje_Status", "\"status\" IN (0, 1, 2)"));

            builder.HasKey(p => new { p.UserId, p.ListingId });

            builder.Property(p => p.UserId).HasColumnName("korisnikID");
            builder.Property(p => p.ListingId).HasColumnName("oglasID");

            builder.Property(p => p.CreatedAt)
                .HasColumnName("datumKreiranja")
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.Property(p => p.Status)
                .HasColumnName("status")
                .IsRequired()
                .HasConversion<int>();

            builder.Property(p => p.ResolvedAt).HasColumnName("datumRazresavanja");

            builder.HasIndex(p => p.ListingId);

            builder.HasOne(p => p.Listing)
                .WithMany(o => o.Claims)
                .HasForeignKey(p => p.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
