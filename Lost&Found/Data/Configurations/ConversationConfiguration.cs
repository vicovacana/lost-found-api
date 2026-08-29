using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable("Razgovor", t =>
                t.HasCheckConstraint("CK_Razgovor_Status", "\"statusRazgovora\" IN (0, 1)"));

            builder.HasKey(r => r.ConversationId);
            builder.Property(r => r.ConversationId).HasColumnName("razgovorID");

            builder.Property(r => r.CreatedAt)
                .HasColumnName("datumKreiranja")
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.Property(r => r.Status)
                .HasColumnName("statusRazgovora")
                .IsRequired()
                .HasConversion<int>();

            builder.Property(r => r.ListingId).HasColumnName("oglasID").IsRequired();

            builder.HasIndex(r => r.ListingId).IsUnique();

            builder.HasOne(r => r.Listing)
                .WithOne(o => o.Conversation)
                .HasForeignKey<Conversation>(r => r.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
