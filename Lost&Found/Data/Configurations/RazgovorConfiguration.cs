using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class RazgovorConfiguration : IEntityTypeConfiguration<Razgovor>
    {
        public void Configure(EntityTypeBuilder<Razgovor> builder)
        {
            builder.ToTable("Razgovor", t =>
                t.HasCheckConstraint("CK_Razgovor_Status", "\"statusRazgovora\" IN (0, 1)"));

            builder.HasKey(r => r.RazgovorId);
            builder.Property(r => r.RazgovorId).HasColumnName("razgovorID");

            builder.Property(r => r.DatumKreiranja)
                .HasColumnName("datumKreiranja")
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.Property(r => r.StatusRazgovora)
                .HasColumnName("statusRazgovora")
                .IsRequired()
                .HasConversion<int>();

            builder.Property(r => r.OglasId).HasColumnName("oglasID").IsRequired();

            builder.HasIndex(r => r.OglasId).IsUnique();

            builder.HasOne(r => r.Oglas)
                .WithOne(o => o.Razgovor)
                .HasForeignKey<Razgovor>(r => r.OglasId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
