namespace SharboAPI.Domain.Tests;

public class GroupParticipantRoleTests
{
	[Fact]
	public void Create_ShouldInitializeGroupParticipantRoleCorrectly()
	{
		// Arrange
		var groupParticipantId = Guid.NewGuid();
		var roleId = 23;

		var expectedGroupParticipantRole = new
		{
			GroupParticipantId = groupParticipantId,
			RoleId = 23,
		};

		// Act
		var result = GroupParticipantRole.Create(groupParticipantId, roleId);

		// Assert
		result.Should().BeEquivalentTo(expectedGroupParticipantRole);
	}

	[Fact]
	public void Update_ShouldUpdateGroupParticipantRoleCorrectly()
	{
		// Arrange
		var roleId = 2;
		var updatedRoleId = 5;
		var groupParticipantId = Guid.NewGuid();

		var groupParticipantRoleToUpdate = GroupParticipantRole.Create(groupParticipantId, roleId);

		// Act
		groupParticipantRoleToUpdate.Update(updatedRoleId);

		// Assert
		groupParticipantRoleToUpdate.RoleId.Should().Be(updatedRoleId);
	}
}
