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
        var createdById = Guid.NewGuid();
        var participants = TestDataFactory.CreateUsers(2);
        const string imagePath = "test_image_path.jpg";
        const string text = "Test meme text";

        var expectedMeme = new
        {
            ImagePath = imagePath,
            Text = text
        };

        // Act
        var meme = Meme.Create(createdById, participants, imagePath, text);

        // Assert
        meme.Should().NotBeNull();
        meme.Should().BeEquivalentTo(expectedMeme);
    }

    [Fact]
    public void Update_ShouldUpdateMemeCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = TestDataFactory.CreateUsers(1);
        var meme = Meme.Create(createdById, initialParticipants, "initial_image.jpg", "Initial text");

        var modifiedById = Guid.NewGuid();
        var updatedParticipants = TestDataFactory.CreateUsers(2);
        const string updatedImagePath = "updated_image.jpg";
        const string updatedText = "Updated meme text";

        var expectedMeme = new
        {
            ImagePath = updatedImagePath,
            Text = updatedText
        };

        // Act
        Meme.Update(meme, modifiedById, updatedParticipants, updatedImagePath, updatedText);

        // Assert
        meme.Should().BeEquivalentTo(expectedMeme);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Meme? meme = null;
        var modifiedById = Guid.NewGuid();
        var participants = TestDataFactory.CreateUsers(1);
        const string imagePath = "updated_image.jpg";
        const string text = "Updated meme text";

        // Act
        var act = () => Meme.Update(meme, modifiedById, participants, imagePath, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
