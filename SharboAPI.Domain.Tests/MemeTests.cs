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
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com", "123456789"),
            User.Create("User2", "user2@example.com", "123456789")
        };
        const string imagePath = "test_image_path.jpg";
        const string text = "Test meme text";

        // Act
        var meme = Meme.Create(createdById, participants, imagePath, text);

        // Assert
        meme.Should().NotBeNull();
        meme.Entry.Should().NotBeNull();
        meme.Entry.CreatedById.Should().Be(createdById);
        meme.Entry.Participants.Should().BeEquivalentTo(participants);
        meme.ImagePath.Should().Be(imagePath);
        meme.Text.Should().Be(text);
    }

    [Fact]
    public void Update_ShouldUpdateMemeCorrectly()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        var initialParticipants = new List<User>
        {
            User.Create("User1", "user1@example.com", "123456789")
        };
        var meme = Meme.Create(createdById, initialParticipants, "initial_image.jpg", "Initial text");

        var modifiedById = Guid.NewGuid();
        var updatedParticipants = new List<User>
        {
            User.Create("User2", "user2@example.com", "123456789"),
            User.Create("User3", "user3@example.com", "123456789")
        };
        const string updatedImagePath = "updated_image.jpg";
        const string updatedText = "Updated meme text";

        // Act
        Meme.Update(meme, modifiedById, updatedParticipants, updatedImagePath, updatedText);

        // Assert
        meme.Entry.LastModifiedById.Should().Be(modifiedById);
        meme.Entry.Participants.Should().BeEquivalentTo(updatedParticipants);
        meme.ImagePath.Should().Be(updatedImagePath);
        meme.Text.Should().Be(updatedText);
    }

    [Fact]
    public void Update_ShouldThrowException_WhenEntityIsNull()
    {
        // Arrange
        Meme meme = null;
        var modifiedById = Guid.NewGuid();
        var participants = new List<User>
        {
            User.Create("User1", "user1@example.com", "123456789")
        };
        const string imagePath = "updated_image.jpg";
        const string text = "Updated meme text";

        // Act
        var act = () => Meme.Update(meme, modifiedById, participants, imagePath, text);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}
