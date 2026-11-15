using FluentValidation;
using FluentValidation.Results;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.User;
using SharboAPI.Application.Services;

namespace SharboAPI.Application.Tests.Services;

// xUnit creates a new instance per test, so every test gets fresh mocks.
public class UserServiceTests
{
	private const string CurrentUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";
	private const string CurrentUserEmail = "tester@example.com";

	private readonly Mock<IUserRepository> _userRepositoryMock = new();
	private readonly Mock<IValidator<CreateProfileRequest>> _createValidatorMock = new();
	private readonly Mock<ICurrentUser> _currentUserMock = new();
	private readonly UserService _userService;

	public UserServiceTests()
	{
		_currentUserMock.Setup(mock => mock.UserId).Returns(CurrentUserId);
		_currentUserMock.Setup(mock => mock.Email).Returns(CurrentUserEmail);

		_createValidatorMock
			.Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateProfileRequest>>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(new ValidationResult());

		_userService = new UserService(_userRepositoryMock.Object, _createValidatorMock.Object, _currentUserMock.Object);
	}

	#region GetCurrentAsync

	[Fact]
	public async Task GetCurrentAsync_ForSignedInUser_ReturnsProfile_WhenProfileExists()
	{
		// Arrange
		SetupExistingProfile("Tester");

		// Act
		var result = await _userService.GetCurrentAsync(CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Should().Be(new UserDetailsResult(CurrentUserId, CurrentUserEmail, "Tester"));
	}

	[Fact]
	public async Task GetCurrentAsync_ForSignedInUser_ReturnsNotFoundResult_WhenProfileDoesNotExist()
	{
		// Act
		var result = await _userService.GetCurrentAsync(CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.NotFound);
	}

	#endregion

	#region CreateCurrentAsync

	[Fact]
	public async Task CreateCurrentAsync_ForCreateProfileRequest_CreatesProfileWithIdAndEmailFromToken()
	{
		// Arrange
		var request = new CreateProfileRequest("Tester");

		// Act
		var result = await _userService.CreateCurrentAsync(request, CancellationToken.None);

		// Assert
		result.IsSuccess.Should().BeTrue();
		result.Value.Should().Be(new UserDetailsResult(CurrentUserId, CurrentUserEmail, "Tester"));
		_userRepositoryMock.Verify(mock => mock.AddAsync(
				It.Is<User>(u => u.Id == CurrentUserId && u.Email == CurrentUserEmail && u.Nickname == "Tester"),
				It.IsAny<CancellationToken>()),
			Times.Once);
	}

	[Fact]
	public async Task CreateCurrentAsync_ForCreateProfileRequest_ReturnsConflictResult_WhenProfileAlreadyExists()
	{
		// Arrange
		SetupExistingProfile("Existing");

		// Act
		var result = await _userService.CreateCurrentAsync(new CreateProfileRequest("Tester"), CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.Conflict);
		_userRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task CreateCurrentAsync_ForCreateProfileRequest_ReturnsBadRequestResult_WhenTokenHasNoEmail()
	{
		// Arrange
		_currentUserMock.Setup(mock => mock.Email).Returns((string?)null);

		// Act
		var result = await _userService.CreateCurrentAsync(new CreateProfileRequest("Tester"), CancellationToken.None);

		// Assert
		result.IsFailure.Should().BeTrue();
		result.Error.Type.Should().Be(ErrorType.BadRequest);
		_userRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	[Fact]
	public async Task CreateCurrentAsync_ForCreateProfileRequest_ThrowsValidationException_WhenValidationFailed()
	{
		// Arrange
		// ValidateAndThrowAsync relies on the validator itself to throw, so the mock throws like a real validator would.
		_createValidatorMock
			.Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateProfileRequest>>(), It.IsAny<CancellationToken>()))
			.ThrowsAsync(new ValidationException([new ValidationFailure("Nickname", "Nickname is required")]));

		// Act
		var act = async () => await _userService.CreateCurrentAsync(new CreateProfileRequest(""), CancellationToken.None);

		// Assert
		await act.Should().ThrowAsync<ValidationException>();
		_userRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
	}

	#endregion

	#region Test_Factory_Methods

	private void SetupExistingProfile(string nickname)
		=> _userRepositoryMock
			.Setup(mock => mock.GetByIdAsync(CurrentUserId, It.IsAny<CancellationToken>()))
			.ReturnsAsync(User.Create(CurrentUserId, nickname, CurrentUserEmail));

	#endregion
}
