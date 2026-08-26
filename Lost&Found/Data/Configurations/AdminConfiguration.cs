using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class AdminConfiguration : IEntityTypeConfiguration<Admin>
    {
        public void Configure(EntityTypeBuilder<Admin> builder)
        {
            builder.HasMany(a => a.NadgledaniOglasi)
                .WithOne(o => o.Admin)
                .HasForeignKey(o => o.AdminId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasData(new Admin
            {
                KorisnikId = 1,
                KorisnickoIme = "admin",
                Email = "admin@lostfound.local",
                LozinkaHash = "$2a$11$dp3ZWPpuZlt46loF6AoALutXdQDkMvmCRkRm6Kk5UZKbIRbtFEd8a",
                VremeKreiranja = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
        }
    }
}
