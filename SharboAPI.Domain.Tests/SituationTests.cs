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
        var createdByEmail = "email";
        var participants = TestDataFactory.CreateUsers(2);
        const string text = "This is a test situation.";

        var expectedSituation = new
        {
            Text = text
        };

        // Act
        var situation = Situation.Create(createdByEmail, participants, text);

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
        var createdByEmail = "email";
        var initialParticipants = TestDataFactory.CreateUsers(1);
        var situation = Situation.Create(createdByEmail, initialParticipants, "Initial situation text.");

        var modifiedByEmail = "email";
        var updatedParticipants = TestDataFactory.CreateUsers(2);
        const string updatedText = "This is the updated situation text.";

        var expectedSituation = new
        {
            Text = updatedText
        };

        // Act
        Situation.Update(situation, modifiedByEmail, updatedParticipants, updatedText);

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
        Situation? situation = null;
        var modifiedByEmail = "email";
        var participants = TestDataFactory.CreateUsers(1);
        const string text = "Updated situation text.";

        // Act
        var act = () => Situation.Update(situation, modifiedByEmail, participants, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
