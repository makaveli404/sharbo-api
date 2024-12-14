using FluentAssertions;
using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class GroupParticipantsTests
{
	[Fact]
	public void Create_ShouldInitializeGroupParticipantsCorrectly()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		const bool isAdmin = true;

		// Arrange & Act
		var groupParticipant = GroupParticipants.Create(groupId, userId, isAdmin);

		// Assert
		var expectedParticipant = new
		{
			GroupId = groupId,
			UserId = userId,
			IsAdmin = isAdmin
		};
		groupParticipant.Should().BeEquivalentTo(expectedParticipant);
	}

	[Fact]
	public void UpdateAdminStatus_ShouldUpdateAdminFlagCorrectly()
	{
		// Arrange
		var groupId = Guid.NewGuid();
		var userId = Guid.NewGuid();
		const bool initialIsAdmin = false;
		var groupParticipant = GroupParticipants.Create(groupId, userId, initialIsAdmin);

		// Act
		GroupParticipants.UpdateAdminStatus(groupParticipant, true);

		// Assert
		groupParticipant.IsAdmin.Should().BeTrue();
	}

	[Fact]
	public void UpdateAdminStatus_ShouldThrowException_WhenEntityIsNull()
	{
		// Arrange
		GroupParticipants groupParticipant = null;

		// Act
		var act = () => GroupParticipants.UpdateAdminStatus(groupParticipant, true);

		// Assert
		act.Should().Throw<NullReferenceException>();
	}
}
