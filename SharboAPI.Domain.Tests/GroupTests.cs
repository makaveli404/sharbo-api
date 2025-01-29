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
        var createdByEmail = "email";
        var participants = TestDataFactory.CreateGroupParticipants(2);
        const string imagePath = "test_image.jpg";

        var expectedGroup = new
        {
            Name = name,
            ImagePath = imagePath,
            CreatedById = createdByEmail,
            LastModifiedById = createdByEmail,
            GroupParticipants = participants
        };

        // Act
        var group = Group.Create(name, createdByEmail, participants, imagePath);

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
        var createdByEmail = "email";
        var initialParticipants = TestDataFactory.CreateGroupParticipants(1);
        var group = Group.Create("Initial Group", createdByEmail, initialParticipants, "initial_image.jpg");

        const string updatedName = "Updated Group";
        var modifiedByEmail = "email";
        var updatedParticipants = TestDataFactory.CreateGroupParticipants(2);
        const string updatedImagePath = "updated_image.jpg";

        var expectedGroup = new
        {
            Name = updatedName,
            ImagePath = updatedImagePath,
            CreatedById = createdByEmail,
            LastModifiedById = modifiedByEmail,
            GroupParticipants = updatedParticipants
        };

        // Act
        Group.Update(group, updatedName, modifiedByEmail, updatedImagePath, updatedParticipants);

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
        var createdByEmail = "email";
        var initialParticipants = TestDataFactory.CreateGroupParticipants(1);
        var group = Group.Create("Initial Group", createdByEmail, initialParticipants);

        const string updatedName = "Updated Group";
        var modifiedByEmail = "email";
        const string updatedImagePath = "updated_image.jpg";

        var expectedGroup = new
        {
            Name = updatedName,
            ImagePath = updatedImagePath,
            CreatedById = createdByEmail,
            LastModifiedById = modifiedByEmail,
            GroupParticipants = initialParticipants
        };

        // Act
        Group.Update(group, updatedName, modifiedByEmail, updatedImagePath);

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
        var modifiedByEmail = "email";
        var participants = TestDataFactory.CreateGroupParticipants(1);

        // Act
        var act = () => Group.Update(group, updatedName, modifiedByEmail, participants: participants);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
