using FluentAssertions;
using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class MemeTests
{
    [Fact]
    public void Create_ShouldInitializeMemeCorrectly()
    {
        // Arrange
        var createdByEmail = "email";
        var participants = TestDataFactory.CreateUsers(2);
        const string imagePath = "test_image_path.jpg";
        const string text = "Test meme text";

        var expectedMeme = new
        {
            ImagePath = imagePath,
            Text = text
        };

        // Act
        var meme = Meme.Create(createdByEmail, participants, imagePath, text);

        // Assert
        meme.Should().NotBeNull();
        meme.Should().BeEquivalentTo(expectedMeme);
    }

    [Fact]
    public void Update_ShouldUpdateMemeCorrectly()
    {
        // Arrange
        var createdByEmail = "email";
        var initialParticipants = TestDataFactory.CreateUsers(1);
        var meme = Meme.Create(createdByEmail, initialParticipants, "initial_image.jpg", "Initial text");

        var modifiedByEmail = "email";
        var updatedParticipants = TestDataFactory.CreateUsers(2);
        const string updatedImagePath = "updated_image.jpg";
        const string updatedText = "Updated meme text";

        var expectedMeme = new
        {
            ImagePath = updatedImagePath,
            Text = updatedText
        };

        // Act
        Meme.Update(meme, modifiedByEmail, updatedParticipants, updatedImagePath, updatedText);

        // Assert
        meme.Should().BeEquivalentTo(expectedMeme);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Meme? meme = null;
        var modifiedByEmail = "email";
        var participants = TestDataFactory.CreateUsers(1);
        const string imagePath = "updated_image.jpg";
        const string text = "Updated meme text";

        // Act
        var act = () => Meme.Update(meme, modifiedByEmail, participants, imagePath, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
