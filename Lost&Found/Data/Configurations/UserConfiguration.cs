using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Korisnik", t =>
                t.HasCheckConstraint("CK_Korisnik_TipKorisnika", "\"tipKorisnika\" IN (0, 1)"));

            builder.HasKey(k => k.UserId);
            builder.Property(k => k.UserId).HasColumnName("korisnikID");

            builder.Property(k => k.Username)
                .HasColumnName("korisnickoIme")
                .IsRequired()
                .HasMaxLength(50);
            builder.HasIndex(k => k.Username).IsUnique();

            builder.Property(k => k.Email)
                .HasColumnName("email")
                .IsRequired()
                .HasMaxLength(256);
            builder.HasIndex(k => k.Email).IsUnique();

            builder.Property(k => k.PasswordHash)
                .HasColumnName("lozinka")
                .IsRequired();

            builder.Property(k => k.CreatedAt)
                .HasColumnName("vremeKreiranja")
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.HasMany(k => k.Messages)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property<int>("TipKorisnika").HasColumnName("tipKorisnika");
            builder.HasDiscriminator<int>("TipKorisnika")
                .HasValue<StandardUser>(0)
                .HasValue<Admin>(1);
        }
    }
}
