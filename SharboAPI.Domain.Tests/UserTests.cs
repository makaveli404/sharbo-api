using System;
using FluentAssertions;
using SharboAPI.Domain.Models;
using Xunit;

namespace SharboAPI.Domain.Tests;

public class UserTests
{
	[Fact]
	public void Create_ShouldInitializeUserCorrectly()
	{
		// Arrange
		const string nickname = "TestUser";
		const string email = "testuser@example.com";

		// Act
		var user = User.Create(nickname, email);

		// Assert
		user.Should().NotBeNull();
		user.Id.Should().NotBeEmpty();
		user.Nickname.Should().Be(nickname);
		user.Email.Should().Be(email);
		user.GroupParticipants.Should().NotBeNull();
		user.GroupParticipants.Should().BeEmpty();
		user.Entries.Should().BeNull();
	}

	[Fact]
	public void Create_ShouldThrowException_WhenNicknameIsNullOrEmpty()
	{
		// Arrange
		string nickname = null;
		const string email = "testuser@example.com";

		// Act
		var act = () => User.Create(nickname, email);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Create_ShouldThrowException_WhenEmailIsNullOrEmpty()
	{
		// Arrange
		const string nickname = "TestUser";
		string email = null;

		// Act
		var act = () => User.Create(nickname, email);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}
}
