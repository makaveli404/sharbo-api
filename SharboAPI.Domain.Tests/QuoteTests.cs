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
        var createdByEmail = "email";
        var participants = TestDataFactory.CreateUsers(2);
        const string text = "This is a test quote.";

        var expectedQuote = new
        {
            Text = text
        };

        // Act
        var quote = Quote.Create(createdByEmail, participants, text);

        // Assert
        quote.Should().NotBeNull();
        quote.Should().BeEquivalentTo(expectedQuote);
    }

    [Fact]
    public void Update_ShouldUpdateQuoteCorrectly()
    {
        // Arrange
        var createdByEmail = "email";
        var initialParticipants = TestDataFactory.CreateUsers(1);
        var quote = Quote.Create(createdByEmail, initialParticipants, "Initial quote text.");

        var modifiedByEmail = "email";
        var updatedParticipants = TestDataFactory.CreateUsers(2);
        const string updatedText = "This is the updated quote text.";

        var expectedQuote = new
        {
            Text = updatedText
        };

        // Act
        Quote.Update(quote, modifiedByEmail, updatedParticipants, updatedText);

        // Assert
        quote.Should().BeEquivalentTo(expectedQuote);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Quote? quote = null;
        var modifiedByEmail = "email";
        var participants = TestDataFactory.CreateUsers(1);
        const string text = "Updated quote text.";

        // Act
        var act = () => Quote.Update(quote, modifiedByEmail, participants, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
