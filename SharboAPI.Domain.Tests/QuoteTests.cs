using System;
using System.Collections.Generic;
using FluentAssertions;
using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class QuoteTests
{
    [Fact]
    public void Create_ShouldInitializeQuoteCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com"),
            User.Create("User2", "user2@example.com")
        };
        const string text = "This is a test quote.";

        // Act
        var quote = Quote.Create(createdById, participants, text);

        // Assert
        quote.Should().NotBeNull();
        quote.Entry.Should().NotBeNull();
        quote.Entry.CreatedById.Should().Be(createdById);
        quote.Entry.Participants.Should().BeEquivalentTo(participants);
        quote.Text.Should().Be(text);
    }

    [Fact]
    public void Update_ShouldUpdateQuoteCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = new List<User>
        {
            User.Create("User1", "user1@example.com")
        };
        var quote = Quote.Create(createdById, initialParticipants, "Initial quote text.");

        var modifiedById = Guid.NewGuid();
        var updatedParticipants = new List<User>
        {
            User.Create("User2", "user2@example.com"),
            User.Create("User3", "user3@example.com")
        };
        const string updatedText = "This is the updated quote text.";

        // Act
        Quote.Update(quote, modifiedById, updatedParticipants, updatedText);

        // Assert
        quote.Entry.LastModifiedById.Should().Be(modifiedById);
        quote.Entry.Participants.Should().BeEquivalentTo(updatedParticipants);
        quote.Text.Should().Be(updatedText);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Quote quote = null;
        var modifiedById = Guid.NewGuid();
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com")
        };
        const string text = "Updated quote text.";

        // Act
        var act = () => Quote.Update(quote, modifiedById, participants, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
