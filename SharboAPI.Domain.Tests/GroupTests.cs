using System;
using System.Collections.Generic;
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
        var participants = new List<GroupParticipants>
        {
            GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), true),
            GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), false)
        };
        const string imagePath = "test_image.jpg";

        // Act
        var group = Group.Create(name, createdById, participants, imagePath);

        // Assert
        group.Should().NotBeNull();
        group.Id.Should().NotBeEmpty();
        group.Name.Should().Be(name);
        group.ImagePath.Should().Be(imagePath);
        group.CreatedById.Should().Be(createdById);
        group.LastModifiedById.Should().Be(createdById);
        group.GroupParticipants.Should().HaveCount(2);
        group.GroupParticipants.Should().BeEquivalentTo(participants, options => options.Excluding(p => p.Group));
        group.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldUpdateGroupCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = new List<GroupParticipants>
        {
            GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), true)
        };
        var group = Group.Create("Initial Group", createdById, initialParticipants, "initial_image.jpg");

        const string updatedName = "Updated Group";
        var modifiedById = Guid.NewGuid();
        var updatedParticipants = new List<GroupParticipants>
        {
            GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), false),
            GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), true)
        };
        const string updatedImagePath = "updated_image.jpg";

        // Act
        Group.Update(group, updatedName, modifiedById, updatedImagePath, updatedParticipants);

        // Assert
        group.Name.Should().Be(updatedName);
        group.ImagePath.Should().Be(updatedImagePath);
        group.LastModifiedById.Should().Be(modifiedById);
        group.GroupParticipants.Should().HaveCount(2);
        group.GroupParticipants.Should().BeEquivalentTo(updatedParticipants, options => options.Excluding(p => p.Group));
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldNotChangeParticipants_WhenParticipantsAreNull()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = new List<GroupParticipants>
        {
            GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), true)
        };
        var group = Group.Create("Initial Group", createdById, initialParticipants);

        const string updatedName = "Updated Group";
        var modifiedById = Guid.NewGuid();
        const string updatedImagePath = "updated_image.jpg";

        // Act
        Group.Update(group, updatedName, modifiedById, updatedImagePath);

        // Assert
        group.Name.Should().Be(updatedName);
        group.ImagePath.Should().Be(updatedImagePath);
        group.LastModifiedById.Should().Be(modifiedById);
        group.GroupParticipants.Should().BeEquivalentTo(initialParticipants, options => options.Excluding(p => p.Group)); // No change
        group.ModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Group group = null;
        const string updatedName = "Updated Group";
        var modifiedById = Guid.NewGuid();
        var participants = new List<GroupParticipants>
        {
            GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), true)
        };

        // Act
        var act = () => Group.Update(group, updatedName, modifiedById, participants: participants);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
