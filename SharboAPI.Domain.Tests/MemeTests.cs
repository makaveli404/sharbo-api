namespace SharboAPI.Domain.Tests;

public class MemeTests
{
    [Fact]
    public void Create_ShouldInitializeMemeCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        const string IMAGE_PATH = "test_image_path.jpg";
        const string TEXT = "Test meme text";

        var expectedMeme = new
        {
            ImagePath = IMAGE_PATH,
            Text = TEXT
        };

        // Act
        var meme = Meme.Create(createdById, IMAGE_PATH, TEXT);

        // Assert
        meme.Should().NotBeNull();
        meme.Should().BeEquivalentTo(expectedMeme);
    }

    [Fact]
    public void Update_ShouldUpdateMemeCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var meme = Meme.Create(createdById, "initial_image.jpg", "Initial text");

        var modifiedById = Guid.NewGuid();
        const string UPDATED_IMAGE_PATH = "updated_image.jpg";
        const string UPDATED_TEXT = "Updated meme text";

        var expectedMeme = new
        {
            CreatedById = createdById,
            LastModifiedById = modifiedById,
            ImagePath = UPDATED_IMAGE_PATH,
            Text = UPDATED_TEXT
        };

        // Act
        Meme.Update(meme, modifiedById, UPDATED_IMAGE_PATH, UPDATED_TEXT);

        // Assert
        meme.Should().BeEquivalentTo(expectedMeme);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Meme? meme = null;
        var modifiedById = Guid.NewGuid();
        const string IMAGE_PATH = "updated_image.jpg";
        const string TEXT = "Updated meme text";

        // Act
        var act = () => Meme.Update(meme, modifiedById, IMAGE_PATH, TEXT);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
