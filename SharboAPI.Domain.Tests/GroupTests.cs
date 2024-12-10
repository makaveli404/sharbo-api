using FluentAssertions;
using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class GroupTests
{
    [Fact]
    public void Create_ShouldInitializeGroupCorrectly()
    {
        // Arrange
        const string name = "Test Group";
        var createdById = Guid.NewGuid();
        var participants = TestDataFactory.CreateGroupParticipants(2);
        const string imagePath = "test_image.jpg";

        var expectedGroup = new
        {
            Name = name,
            ImagePath = imagePath,
            CreatedById = createdById,
            LastModifiedById = createdById,
            GroupParticipants = participants
        };

        // Act
        var group = Group.Create(name, createdById, participants, imagePath);

        // Assert
        group.Should().NotBeNull();
        group.Should().BeEquivalentTo(expectedGroup, options => options
            .ExcludingMissingMembers()
            .Excluding(g => g.GroupParticipants));
        group.GroupParticipants.Should().HaveCount(2);
        group.GroupParticipants.Should().BeEquivalentTo(participants);
        group.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldUpdateGroupCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = TestDataFactory.CreateGroupParticipants(1);
        var group = Group.Create("Initial Group", createdById, initialParticipants, "initial_image.jpg");

        const string updatedName = "Updated Group";
        var modifiedById = Guid.NewGuid();
        var updatedParticipants = TestDataFactory.CreateGroupParticipants(2);
        const string updatedImagePath = "updated_image.jpg";

        var expectedGroup = new
        {
            Name = updatedName,
            ImagePath = updatedImagePath,
            CreatedById = createdById,
            LastModifiedById = modifiedById,
            GroupParticipants = updatedParticipants
        };

        // Act
        Group.Update(group, updatedName, modifiedById, updatedImagePath, updatedParticipants);

        // Assert
        group.Should().NotBeNull();
        group.Should().BeEquivalentTo(expectedGroup, options => options
            .ExcludingMissingMembers()
            .Excluding(g => g.GroupParticipants));
        group.GroupParticipants.Should().HaveCount(2);
        group.GroupParticipants.Should().BeEquivalentTo(updatedParticipants);
        group.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldNotChangeParticipants_WhenParticipantsAreNull()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = TestDataFactory.CreateGroupParticipants(1);
        var group = Group.Create("Initial Group", createdById, initialParticipants);

        const string updatedName = "Updated Group";
        var modifiedById = Guid.NewGuid();
        const string updatedImagePath = "updated_image.jpg";

        var expectedGroup = new
        {
            Name = updatedName,
            ImagePath = updatedImagePath,
            CreatedById = createdById,
            LastModifiedById = modifiedById,
            GroupParticipants = initialParticipants
        };

        // Act
        Group.Update(group, updatedName, modifiedById, updatedImagePath);

        // Assert
        group.Should().NotBeNull();
        group.Should().BeEquivalentTo(expectedGroup, options => options
            .ExcludingMissingMembers()
            .Excluding(g => g.GroupParticipants));
        group.GroupParticipants.Should().HaveCount(1);
        group.GroupParticipants.Should().BeEquivalentTo(initialParticipants);
        group.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Group group = null;
        const string updatedName = "Updated Group";
        var modifiedById = Guid.NewGuid();
        var participants = TestDataFactory.CreateGroupParticipants(1);

        // Act
        var act = () => Group.Update(group, updatedName, modifiedById, participants: participants);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
