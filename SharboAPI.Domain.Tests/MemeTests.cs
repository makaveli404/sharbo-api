namespace SharboAPI.Domain.Tests;

public class MemeTests
{
	[Fact]
	public void Create_ShouldInitializeMemeCorrectly()
	{
		// Arrange
		GroupParticipant createdBy = CreateGroupParticipantWithDefaults();
		const string IMAGE_PATH = "test_image_path.jpg";
		const string TEXT = "Test meme text";

		var expectedMeme = new
		{
			ImagePath = IMAGE_PATH,
			Text = TEXT
		};

		// Act
		var meme = Meme.Create(createdBy, IMAGE_PATH, TEXT);

		// Assert
		meme.Should().NotBeNull();
		meme.Should().BeEquivalentTo(expectedMeme);
	}

	[Fact]
	public void UpdateText_ShouldUpdateMemeCorrectly()
	{
		// Arrange
		GroupParticipant createdBy = CreateGroupParticipantWithDefaults();
		var meme = Meme.Create(createdBy, "initial_image.jpg", "Initial text");

		GroupParticipant modifiedBy = CreateGroupParticipantWithDefaults();
		const string UPDATED_TEXT = "Updated meme text";

		var expectedMeme = new
		{
			CreatedById = createdBy,
			LastModifiedById = modifiedBy,
			Text = UPDATED_TEXT
		};

		// Act
		meme.UpdateText(modifiedBy, UPDATED_TEXT);

		// Assert
		meme.Should().BeEquivalentTo(expectedMeme);
	}

	[Fact]
	public void UpdateText_ShouldThrowException_WhenEntityIsNull()
	{
		// Arrange
		Meme? meme = null;
		var modifiedBy = CreateGroupParticipantWithDefaults();
		const string UPDATED_TEXT = "Updated meme text";

		// Act
		var act = () => meme.UpdateText(modifiedBy, UPDATED_TEXT);

		// Assert
		act.Should().Throw<NullReferenceException>();
	}

	private GroupParticipant CreateGroupParticipantWithDefaults(Guid? groupId = null, Guid? userId = null)
		=> GroupParticipant.Create(groupId ?? Guid.NewGuid(), userId ?? Guid.NewGuid());
}
