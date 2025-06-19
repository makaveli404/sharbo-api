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
            .WithMany(gp => gp.CreatedEntries)
            .HasForeignKey(e => e.CreatedById);

        builder
            .HasOne(e => e.LastModifiedBy)
            .WithMany(gp => gp.ModifiedEntries)
            .HasForeignKey(e => e.LastModifiedById);

        builder.ToTable("Entries");
    }
}
