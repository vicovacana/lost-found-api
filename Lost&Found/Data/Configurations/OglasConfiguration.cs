using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class OglasConfiguration : IEntityTypeConfiguration<Oglas>
    {
        public void Configure(EntityTypeBuilder<Oglas> builder)
        {
            builder.ToTable("Oglas", t =>
            {
                t.HasCheckConstraint("CK_Oglas_Tip", "\"tip\" IN (0, 1)");
                t.HasCheckConstraint("CK_Oglas_Latitude", "\"latitude\" IS NULL OR (\"latitude\" >= -90 AND \"latitude\" <= 90)");
                t.HasCheckConstraint("CK_Oglas_Longitude", "\"longitude\" IS NULL OR (\"longitude\" >= -180 AND \"longitude\" <= 180)");
            });

            builder.HasKey(o => o.OglasId);
            builder.Property(o => o.OglasId).HasColumnName("oglasID");

            builder.Property(o => o.Naziv)
                .HasColumnName("naziv")
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(o => o.Opis)
                .HasColumnName("opis")
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(o => o.DatumKreiranja)
                .HasColumnName("datumKreiranja")
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.Property(o => o.Tip)
                .HasColumnName("tip")
                .IsRequired()
                .HasConversion<int>();

            builder.Property(o => o.Kategorija)
                .HasColumnName("kategorija")
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(o => o.Grad)
                .HasColumnName("grad")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Latitude)
                .HasColumnName("latitude")
                .HasColumnType("decimal(10,8)");

            builder.Property(o => o.Longitude)
                .HasColumnName("longitude")
                .HasColumnType("decimal(11,8)");

            builder.Property(o => o.Fotografija)
                .HasColumnName("fotografija")
                .HasMaxLength(500);

            builder.Property(o => o.KreatorId).HasColumnName("kreatorID").IsRequired();
            builder.Property(o => o.AdminId).HasColumnName("adminID");

            builder.HasIndex(o => o.KreatorId);
            builder.HasIndex(o => o.AdminId);
            builder.HasIndex(o => o.Tip);
            builder.HasIndex(o => o.Kategorija);
            builder.HasIndex(o => o.Grad);
        }
    }
}
