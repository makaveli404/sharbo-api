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
        var participants = TestDataFactory.CreateUsers(2);
        const string text = "This is a test situation.";

        var expectedSituation = new
        {
            Text = text
        };

        // Act
        var situation = Situation.Create(createdById, participants, text);

        // Assert
        situation.Should().NotBeNull();
        situation.Should().BeEquivalentTo(expectedSituation);
        situation.Entry.Participants.Should().HaveCount(2);
        situation.Entry.Participants.Should().BeEquivalentTo(participants);
        situation.Entry.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        situation.Entry.LastModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldUpdateSituationCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = TestDataFactory.CreateUsers(1);
        var situation = Situation.Create(createdById, initialParticipants, "Initial situation text.");

        var modifiedById = Guid.NewGuid();
        var updatedParticipants = TestDataFactory.CreateUsers(2);
        const string updatedText = "This is the updated situation text.";

        var expectedSituation = new
        {
            Text = updatedText
        };

        // Act
        Situation.Update(situation, modifiedById, updatedParticipants, updatedText);

        // Assert
        situation.Should().NotBeNull();
        situation.Should().BeEquivalentTo(expectedSituation);
        situation.Entry.Participants.Should().HaveCount(2);
        situation.Entry.Participants.Should().BeEquivalentTo(updatedParticipants);
        situation.Entry.CreationDate.Date.Should().Be(DateTime.UtcNow.Date);
        situation.Entry.LastModificationDate.Date.Should().Be(DateTime.UtcNow.Date);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Situation situation = null;
        var modifiedById = Guid.NewGuid();
        var participants = TestDataFactory.CreateUsers(1);
        const string text = "Updated situation text.";

        // Act
        var act = () => Situation.Update(situation, modifiedById, participants, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
