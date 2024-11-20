using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class EntryTests
{
	[Fact]
	public void Create_ShouldInitializeEntryCorrectly()
	{
		// Arrange
		var creatorId = Guid.NewGuid();
		var participants = new List<User>
		{
			User.Create("User1", "user1@example.com"),
			User.Create("User2", "user2@example.com")
		};

		// Act
		var entry = Entry.Create(creatorId, participants);

		// Assert
		Assert.NotNull(entry);
		Assert.Equal(creatorId, entry.CreatedById);
		Assert.Equal(creatorId, entry.LastModifiedById);
		Assert.Equal(participants, entry.Participants);
		Assert.Equal(DateTime.UtcNow.Date, entry.CreationDate.Date);
		Assert.Equal(DateTime.UtcNow.Date, entry.LastModificationDate.Date);
	}

	[Fact]
	public void Update_ShouldUpdateEntryCorrectly()
	{
		// Arrange
		var creatorId = Guid.NewGuid();
		var initialParticipants = new List<User>
		{
			User.Create("User1", "user1@example.com")
		};
		var entry = Entry.Create(creatorId, initialParticipants);

		var modifierId = Guid.NewGuid();
		var updatedParticipants = new List<User>
		{
			User.Create("User2", "user2@example.com"),
			User.Create("User3", "user3@example.com")
		};

		// Act
		Entry.Update(entry, modifierId, updatedParticipants);

		// Assert
		Assert.Equal(modifierId, entry.LastModifiedById);
		Assert.Equal(updatedParticipants, entry.Participants);
		Assert.Equal(DateTime.UtcNow.Date, entry.LastModificationDate.Date);
	}

	[Fact]
	public void Update_ShouldThrowException_WhenEntityIsNull()
	{
		// Arrange
		Entry entry = null;
		var modifierId = Guid.NewGuid();
		var participants = new List<User>
		{
			User.Create("User1", "user1@example.com")
		};

		// Act & Assert
		Assert.Throws<NullReferenceException>(() => Entry.Update(entry, modifierId, participants));
	}
}
