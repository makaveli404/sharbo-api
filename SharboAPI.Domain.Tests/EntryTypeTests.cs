using SharboAPI.Domain.Enums;

namespace SharboAPI.Domain.Tests;

public class EntryTypeTests
{
	[Fact]
	public void MemeCreate_SetsMemeType()
	{
		// Act
		var meme = Meme.Create(Guid.NewGuid(), "image.jpg");

		// Assert
		meme.Type.Should().Be(EntryType.Meme);
	}

	[Fact]
	public void QuoteCreate_SetsQuoteType()
	{
		// Act
		var quote = Quote.Create(Guid.NewGuid(), "Quote text");

		// Assert
		quote.Type.Should().Be(EntryType.Quote);
	}

	[Fact]
	public void SituationCreate_SetsSituationType()
	{
		// Act
		var situation = Situation.Create(Guid.NewGuid(), "Situation text");

		// Assert
		situation.Type.Should().Be(EntryType.Situation);
	}
}
