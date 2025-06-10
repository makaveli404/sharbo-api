using SharboAPI.Domain.Enums;

namespace SharboAPI.Domain.Tests;

public class RoleTests
{
    [Fact]
    public void Create_ShouldInitializeRoleCorrectly()
    {
        // Arrange 
        var roleType = RoleType.Participant;
        var name = "Participant";

        var expectedRole = new
        {
            RoleType = RoleType.Participant,
            Name = "Participant"
        };

        // Act
        var result = Role.Create(roleType, name);

        // Assert
        result.Should().BeEquivalentTo(expectedRole);
    }

    [Fact]
    public void Create_ShouldUpdateRoleCorrectly()
    {
        // Arrange
        var roleToUpdate = Role.Create(RoleType.Participant, "Participant");
        var roleType = RoleType.Admin;
        var name = "Admin";

        // Act
        Role.Update(roleToUpdate, roleType, name);

        // Assert
        roleToUpdate.RoleType.Should().Be(roleType);
        roleToUpdate.Name.Should().Be(name);
    }
}
