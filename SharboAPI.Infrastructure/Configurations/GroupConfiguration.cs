using SharboAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SharboAPI.Infrastructure.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder
            .HasOne(g => g.CreatedBy)
            .WithMany()
            .HasForeignKey(g => g.CreatedByEmail)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(g => g.LastModifiedBy)
            .WithMany()
            .HasForeignKey(g => g.LastModifiedByEmail)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
