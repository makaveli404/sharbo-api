using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder
            .HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedByEmail)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(e => e.LastModifiedBy)
            .WithMany()
            .HasForeignKey(e => e.LastModifiedByEmail)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(e => e.Participants)
            .WithMany(u => u.Entries)
            .UsingEntity(e => e.ToTable("EntryParticipants"));
    }
}
