using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class GroupParticipantConfiguration : IEntityTypeConfiguration<GroupParticipant>
{
    public void Configure(EntityTypeBuilder<GroupParticipant> builder)
    {
        builder
            .HasOne(gp => gp.User)
            .WithMany()
            .HasForeignKey(gp => gp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(gp => gp.Group)
            .WithMany(g => g.GroupParticipants)
            .HasForeignKey(gp => gp.GroupId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
