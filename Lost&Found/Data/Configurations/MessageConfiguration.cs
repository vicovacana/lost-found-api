using Lost_Found.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lost_Found.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Poruka");

            builder.HasKey(p => p.MessageId);
            builder.Property(p => p.MessageId).HasColumnName("porukaID");

            builder.Property(p => p.UserId).HasColumnName("korisnikID").IsRequired();
            builder.Property(p => p.ConversationId).HasColumnName("razgovorID").IsRequired();

            builder.Property(p => p.CreatedAt)
                .HasColumnName("datumKreiranja")
                .IsRequired()
                .HasDefaultValueSql("now()");

            builder.Property(p => p.Content)
                .HasColumnName("sadrzaj")
                .IsRequired()
                .HasMaxLength(4000);

            builder.HasIndex(p => new { p.ConversationId, p.CreatedAt });

            builder.HasOne(p => p.Conversation)
                .WithMany(r => r.Messages)
                .HasForeignKey(p => p.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
