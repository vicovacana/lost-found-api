using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class ListingConfiguration : IEntityTypeConfiguration<Listing>
    {
        public void Configure(EntityTypeBuilder<Listing> builder)
        {
            builder.ToTable("Oglas", t =>
            {
                t.HasCheckConstraint("CK_Oglas_Tip", "\"tip\" IN (0, 1)");
                t.HasCheckConstraint("CK_Oglas_Latitude", "\"latitude\" IS NULL OR (\"latitude\" >= -90 AND \"latitude\" <= 90)");
                t.HasCheckConstraint("CK_Oglas_Longitude", "\"longitude\" IS NULL OR (\"longitude\" >= -180 AND \"longitude\" <= 180)");
            });

            builder.HasKey(o => o.ListingId);
            builder.Property(o => o.ListingId).HasColumnName("oglasID");

            builder.Property(o => o.Title)
                .HasColumnName("naziv")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.Description)
                .HasColumnName("opis")
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(o => o.CreatedAt)
                .HasColumnName("datumKreiranja")
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.Property(o => o.Type)
                .HasColumnName("tip")
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.Category)
                .HasColumnName("kategorija")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(o => o.City)
                .HasColumnName("grad")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Latitude)
                .HasColumnName("latitude")
                .HasColumnType("decimal(10,8)");

            builder.Property(o => o.Longitude)
                .HasColumnName("longitude")
                .HasColumnType("decimal(11,8)");

            builder.Property(o => o.Photo)
                .HasColumnName("fotografija")
                .HasMaxLength(500);

            builder.Property(o => o.LocationDescription)
                .HasColumnName("OpisLokacije");

            builder.Property(o => o.CreatorId).HasColumnName("kreatorID").IsRequired();
            builder.Property(o => o.AdminId).HasColumnName("adminID");

            builder.Property(o => o.IsDeleted)
                .HasColumnName("obrisan")
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasIndex(o => o.CreatorId);
            builder.HasIndex(o => o.AdminId);
            builder.HasIndex(o => o.Type);
            builder.HasIndex(o => o.Category);
            builder.HasIndex(o => o.City);
            builder.HasIndex(o => o.IsDeleted);
        }
    }
}
