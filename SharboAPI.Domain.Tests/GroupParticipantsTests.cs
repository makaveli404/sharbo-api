using System;
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

		// Act
		var groupParticipant = GroupParticipants.Create(groupId, userId, isAdmin);

		// Assert
		groupParticipant.Should().NotBeNull();
		groupParticipant.GroupId.Should().Be(groupId);
		groupParticipant.UserId.Should().Be(userId);
		groupParticipant.IsAdmin.Should().Be(isAdmin);
		groupParticipant.Group.Should().BeNull();
		groupParticipant.User.Should().BeNull();
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
