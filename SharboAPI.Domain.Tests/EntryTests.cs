using FluentAssertions;
using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class EntryTests
{
    [Fact]
    public void Create_ShouldInitializeEntryCorrectly()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var participants = TestDataFactory.CreateUsers();
        var today = DateTime.UtcNow.Date;

        var expectedEntry = new
        {
            CreatedById = creatorId,
            LastModifiedById = creatorId,
            Participants = participants
        };

        // Act
        var entry = Entry.Create(creatorId, participants);

        // Assert
        entry.Should().NotBeNull();
        entry.Should().BeEquivalentTo(expectedEntry, options => options
            .Excluding(e => e.Name == nameof(entry.CreationDate))
            .Excluding(e => e.Name == nameof(entry.LastModificationDate)));
        entry.CreationDate.Date.Should().Be(today);
        entry.LastModificationDate.Date.Should().Be(today);
    }

    [Fact]
    public void Update_ShouldUpdateEntryCorrectly()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var initialParticipants = TestDataFactory.CreateUsers(1);
        var entry = Entry.Create(creatorId, initialParticipants);
        var modifierId = Guid.NewGuid();
        var updatedParticipants = TestDataFactory.CreateUsers(2);
        var today = DateTime.UtcNow.Date;

        // Act
        Entry.Update(entry, modifierId, updatedParticipants);

        // Assert
        var expectedEntry = new
        {
            CreatedById = creatorId,
            LastModifiedById = modifierId,
            Participants = updatedParticipants
        };

        entry.Should().NotBeNull();
        entry.Should().BeEquivalentTo(expectedEntry, options => options
            .Excluding(e => e.Name == nameof(entry.CreationDate))
            .Excluding(e => e.Name == nameof(entry.LastModificationDate)));

        entry.CreationDate.Date.Should().Be(today);
        entry.LastModificationDate.Date.Should().Be(today);
    }


    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Entry? entry = null;
        var participants = TestDataFactory.CreateUsers(1);
        var modifierId = Guid.NewGuid();

        // Act
        var act = () => Entry.Update(entry, modifierId, participants);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
