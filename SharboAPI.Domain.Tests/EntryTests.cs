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
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com", "123456789"),
            User.Create("User2", "user2@example.com", "123456789")
        };

        // Act
        var entry = Entry.Create(creatorId, participants);

        // Assert
        entry.Should().NotBeNull();
        entry.CreatedById.Should().Be(creatorId);
        entry.LastModifiedById.Should().Be(creatorId);
        entry.Participants.Should().BeEquivalentTo(participants);
        entry.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        entry.LastModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldUpdateEntryCorrectly()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var initialParticipants = new List<User>
        {
            User.Create("User1", "user1@example.com", "123456789")
        };
        var entry = Entry.Create(creatorId, initialParticipants);

        var modifierId = Guid.NewGuid();
        var updatedParticipants = new List<User>
        {
            User.Create("User2", "user2@example.com", "123456789"),
            User.Create("User3", "user3@example.com", "123456789")
        };

        // Act
        Entry.Update(entry, modifierId, updatedParticipants);

        // Assert
        entry.LastModifiedById.Should().Be(modifierId);
        entry.Participants.Should().BeEquivalentTo(updatedParticipants);
        entry.LastModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Entry entry = null;
        var modifierId = Guid.NewGuid();
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com", "123456789")
        };

        // Act
        var act = () => Entry.Update(entry, modifierId, participants);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
