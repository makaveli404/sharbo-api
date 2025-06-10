namespace SharboAPI.Domain.Tests;

public class QuoteTests
{
    [Fact]
    public void Create_ShouldInitializeQuoteCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        const string TEXT = "This is a test quote.";

        var expectedQuote = new
        {
            Text = TEXT
        };

        // Act
        var quote = Quote.Create(createdById, TEXT);

        // Assert
        quote.Should().BeEquivalentTo(expectedQuote);
    }

    [Fact]
    public void Update_ShouldUpdateQuoteCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var quote = Quote.Create(createdById, "Initial quote text.");

        var modifiedById = Guid.NewGuid();
        const string UPDATED_TEXT = "This is the updated quote text.";

        var expectedQuote = new
        {
            Text = UPDATED_TEXT
        };

        // Act
        Quote.Update(quote, modifiedById, UPDATED_TEXT);

        // Assert
        quote.Should().BeEquivalentTo(expectedQuote);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Quote? quote = null;
        var modifiedById = Guid.NewGuid();
        const string TEXT = "Updated quote text.";

        // Act
        var act = () => Quote.Update(quote, modifiedById, TEXT);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
