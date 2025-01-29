using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class GroupParticipantsConfiguration : IEntityTypeConfiguration<GroupParticipants>
{
    public void Configure(EntityTypeBuilder<GroupParticipants> builder)
    {
        builder.HasKey(gc => new { gc.GroupId, gc.UserEmail });

        builder
            .HasOne(gp => gp.User)
            .WithMany(u => u.GroupParticipants)
            .HasForeignKey(gp => gp.UserEmail)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(gp => gp.Group)
            .WithMany(g => g.GroupParticipants)
            .HasForeignKey(gp => gp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
