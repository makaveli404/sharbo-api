namespace SharboAPI.Domain.Tests;

public class MemeTests
{
	[Fact]
	public void Create_ShouldInitializeMemeCorrectly()
	{
		// Arrange
		Guid createdById = Guid.NewGuid();
        const string imagePath = "test_image_path.jpg";
		const string text = "Test meme text";

		var expectedMeme = new
		{
			ImagePath = imagePath,
			Text = text
		};

		// Act
		var meme = Meme.Create(createdById, imagePath, text);

		// Assert
		meme.Should().NotBeNull();
		meme.Should().BeEquivalentTo(expectedMeme);
	}

	[Fact]
	public void UpdateText_ShouldUpdateMemeCorrectly()
	{
        // Arrange
        Guid createdById = Guid.NewGuid();
        var meme = Meme.Create(createdById, "initial_image.jpg", "Initial text");

		Guid modifiedById = Guid.NewGuid();
        const string updatedText = "Updated meme text";

		var expectedMeme = new
		{
			CreatedById = createdById,
			LastModifiedById = modifiedById,
			Text = updatedText
		};

		// Act
		meme.UpdateText(modifiedById, updatedText);

		// Assert
		meme.Should().BeEquivalentTo(expectedMeme);
	}

	[Fact]
	public void UpdateText_ShouldThrowException_WhenEntityIsNull()
	{
		// Arrange
		Meme? meme = null;
		var modifiedById = Guid.NewGuid();
		const string updatedText = "Updated meme text";

		// Act
		var act = () => meme.UpdateText(modifiedById, updatedText);

		// Assert
		act.Should().Throw<NullReferenceException>();
	}
}
