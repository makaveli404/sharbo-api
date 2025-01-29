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
        var creatorEmail = "email";
        var participants = TestDataFactory.CreateUsers();
        var today = DateTime.UtcNow.Date;

        var expectedEntry = new
        {
            CreatedByEmail = creatorEmail,
            LastModifiedByEmail = creatorEmail,
            Participants = participants
        };

        // Act
        var entry = Entry.Create(creatorEmail, participants);

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
        var creatorEmail = "email";
        var initialParticipants = TestDataFactory.CreateUsers(1);
        var entry = Entry.Create(creatorEmail, initialParticipants);
        var modifierEmail = "email";
        var updatedParticipants = TestDataFactory.CreateUsers(2);
        var today = DateTime.UtcNow.Date;

        // Act
        Entry.Update(entry, modifierEmail, updatedParticipants);

        // Assert
        var expectedEntry = new
        {
            CreatedByEmail = creatorEmail,
            LastModifiedByEmail = modifierEmail,
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
        var modifierEmail = "email";

        // Act
        var act = () => Entry.Update(entry, modifierEmail, participants);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
