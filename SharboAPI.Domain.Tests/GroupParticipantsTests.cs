using SharboAPI.Domain.Enums;

namespace SharboAPI.Domain.Tests;

public class GroupParticipantsTests
{
	[Fact]
	public void Create_ShouldInitializeCorrectly_WhenGroupParticipantRolesIsNull()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		var userId = "fa4e74f5c58448a5acba";
		List<GroupParticipantRole>? roles = null;
		List<GroupParticipantRole> expectedRoles = [];

		// Arrange & Act
		var groupParticipant = GroupParticipant.Create(groupId, userId, roles);

		// Assert
		var expectedParticipant = new
		{
			GroupId = groupId,
			UserId = userId,
			GroupParticipantRoles = expectedRoles
		};

		groupParticipant.Should().BeEquivalentTo(expectedParticipant);
	}

	[Fact]
	public void Create_ShouldInitializeCorrectly_WhenGroupParticipantRolesIsGiven()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		var userId = "fa4e74f5c58448a5acba";
		List<GroupParticipantRole> roles = [
			GroupParticipantRole.Create(Role.Create(RoleType.Participant, "Participant")),
			GroupParticipantRole.Create(Role.Create(RoleType.Moderator, "Moderator")),
			GroupParticipantRole.Create(Role.Create(RoleType.Admin, "Admin"))
		];

		// Arrange & Act
		var groupParticipant = GroupParticipant.Create(groupId, userId, roles);

		// Assert
		var expectedParticipant = new
		{
			GroupId = groupId,
			UserId = userId,
			GroupParticipantRoles = roles
		};

		groupParticipant.Should().BeEquivalentTo(expectedParticipant);
	}
}
