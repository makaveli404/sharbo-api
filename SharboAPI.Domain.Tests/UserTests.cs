namespace SharboAPI.Domain.Tests;

public class UserTests
{
	[Fact]
	public void Create_ShouldInitializeUserCorrectly()
	{
		// Arrange
		string id = "01K31H597YSPQHJ1FB3JA253DJ";
		string nickname = "TestUser";
		string email = "testuser@example.com";

		var expectedUser = new
		{
			Nickname = nickname,
			Email = email
		};

		// Act
		var user = User.Create(id, nickname, email);

		// Assert
		user.Id.Should().NotBe(string.Empty);
		user.Should().BeEquivalentTo(expectedUser);
	}

	[Fact]
	public void Create_ShouldThrowException_WhenNicknameIsNullOrEmpty()
	{
		// Arrange
		string id = "01K31H597YSPQHJ1FB3JA253DJ";
		string? nickname = null;
		string email = "testuser@example.com";

		// Act
		var act = () => User.Create(id, nickname, email);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Create_ShouldThrowException_WhenEmailIsNullOrEmpty()
	{
		// Arrange
		string id = "01K31H597YSPQHJ1FB3JA253DJ";
		string nickname = "TestUser";
		string? email = null;

		// Act
		var act = () => User.Create(id, nickname, email);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Create_ShouldThrowException_WhenProviderIdIsNullOrEmpty()
	{
		// Arrange
		string? id = null;
		string nickname = "TestUser";
		string email = "testuser@example.com";

		// Act
		var act = () => User.Create(id!, nickname, email);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}
}
