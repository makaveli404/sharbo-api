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
        var groupParticipantRoleToUpdate = GroupParticipantRole.Create(23);
        var groupParticipantId = Guid.NewGuid();
        var roleId = 23;

        // Act
        GroupParticipantRole.Update(groupParticipantRoleToUpdate, roleId);

        // Assert
        groupParticipantRoleToUpdate.RoleId.Should().Be(roleId);
    }
}
