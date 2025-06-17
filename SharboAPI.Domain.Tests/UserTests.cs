namespace SharboAPI.Domain.Tests;

public class UserTests
{
	[Fact]
	public void Create_ShouldInitializeUserCorrectly()
	{
		// Arrange
		string nickname = "TestUser";
		string email = "testuser@example.com";
		string password = "123456789";

		var expectedUser = new
		{
			Nickname = nickname,
			Email = email
		};

		// Act
		var user = User.Create(nickname, email, password);

		// Assert
		user.Id.Should().NotBe(Guid.Empty);
		user.Should().BeEquivalentTo(expectedUser);
	}

	[Fact]
	public void Create_ShouldThrowException_WhenNicknameIsNullOrEmpty()
	{
		// Arrange
		string? nickname = null;
		string email = "testuser@example.com";
		string password = "123456789";

		// Act
		var act = () => User.Create(nickname, email, password);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Create_ShouldThrowException_WhenEmailIsNullOrEmpty()
	{
		// Arrange
		string nickname = "TestUser";
		string? email = null;
		string password = "123456789";

		// Act
		var act = () => User.Create(nickname, email, password);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}
}
