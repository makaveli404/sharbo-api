using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Configurations;

public class GroupParticipantRoleConfiguration : IEntityTypeConfiguration<GroupParticipantRole>
{
	public void Configure(EntityTypeBuilder<GroupParticipantRole> builder)
	{
		builder.HasKey(gpr => new { gpr.GroupParticipantId, gpr.RoleId });

		builder
			.HasOne(gpr => gpr.GroupParticipant)
			.WithMany(gp => gp.GroupParticipantRoles)
			.HasForeignKey(gpr => gpr.GroupParticipantId)
			.OnDelete(DeleteBehavior.Cascade);

		builder
			.HasOne(gpr => gpr.Role)
			.WithMany(r => r.GroupParticipantRoles)
			.HasForeignKey(gpr => gpr.RoleId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
