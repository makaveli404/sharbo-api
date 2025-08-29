namespace SharboAPI.Domain.Tests;

public class QuoteTests
{
	[Fact]
	public void Create_ShouldInitializeQuoteCorrectly()
	{
		// Arrange
		var createdById = Guid.NewGuid();
		const string text = "This is a test quote.";

		var expectedQuote = new
		{
			Text = text
		};

		// Act
		var quote = Quote.Create(createdById, text);

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
		const string updatedText = "This is the updated quote text.";

		var expectedQuote = new
		{
			Text = updatedText
		};

		// Act
		quote.Update(modifiedById, updatedText);

		// Assert
		quote.Should().BeEquivalentTo(expectedQuote);
	}

	[Fact]
	public void Update_ShouldThrowException_WhenEntityIsNull()
	{
		// Arrange
		Quote? quote = null;
		var modifiedById = Guid.NewGuid();
		const string text = "Updated quote text.";

		// Act
		var act = () => quote.Update(modifiedById, text);

		// Assert
		act.Should().Throw<NullReferenceException>();
	}
}
