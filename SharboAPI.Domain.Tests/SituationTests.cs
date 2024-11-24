using FluentAssertions;
using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class SituationTests
{
    [Fact]
    public void Create_ShouldInitializeSituationCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com"),
            User.Create("User2", "user2@example.com")
        };
        const string text = "This is a test situation.";

        // Act
        var situation = Situation.Create(createdById, participants, text);

        // Assert
        situation.Should().NotBeNull();
        situation.Entry.Should().NotBeNull();
        situation.Entry.CreatedById.Should().Be(createdById);
        situation.Entry.Participants.Should().BeEquivalentTo(participants);
        situation.Text.Should().Be(text);
    }

    [Fact]
    public void Update_ShouldUpdateSituationCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = new List<User>
        {
            User.Create("User1", "user1@example.com")
        };
        var situation = Situation.Create(createdById, initialParticipants, "Initial situation text.");

        var modifiedById = Guid.NewGuid();
        var updatedParticipants = new List<User>
        {
            User.Create("User2", "user2@example.com"),
            User.Create("User3", "user3@example.com")
        };
        const string updatedText = "This is the updated situation text.";

        // Act
        Situation.Update(situation, modifiedById, updatedParticipants, updatedText);

        // Assert
        situation.Entry.LastModifiedById.Should().Be(modifiedById);
        situation.Entry.Participants.Should().BeEquivalentTo(updatedParticipants);
        situation.Text.Should().Be(updatedText);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Situation situation = null;
        var modifiedById = Guid.NewGuid();
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com")
        };
        const string text = "Updated situation text.";

        // Act
        var act = () => Situation.Update(situation, modifiedById, participants, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
