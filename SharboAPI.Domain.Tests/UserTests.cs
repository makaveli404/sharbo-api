using FluentAssertions;
using Moq;
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
		const string password = "123456789";

		var expectedUser = new
		{
			Nickname = nickname,
			Email = email
		};

		// Act
		var user = User.Create(nickname, email, password);

		// Assert
		user.Id.Should().NotBe(Guid.Empty);
		user.Should().NotBeNull();
		user.Should().BeEquivalentTo(expectedUser);
	}

	[Fact]
	public void Create_ShouldThrowException_WhenNicknameIsNullOrEmpty()
	{
		// Arrange
		string? nickname = null;
		const string email = "testuser@example.com";
		const string password = "123456789";

		// Act
		var act = () => User.Create(nickname, email, password);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Create_ShouldThrowException_WhenEmailIsNullOrEmpty()
	{
		// Arrange
		const string nickname = "TestUser";
		string? email = null;
		string password = "123456789";

		// Act
		var act = () => User.Create(nickname, email, password);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}
}
