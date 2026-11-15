using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Meme;
using SharboAPI.Application.Services;
using FluentValidation;
using FluentValidation.Results;

namespace SharboAPI.Application.Tests.Services;

// xUnit creates a new instance per test, so every test gets fresh mocks.
public class MemeServiceTests
{
    private const string CurrentUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

    private readonly Mock<IMemeRepository> _memeRepositoryMock = new();
    private readonly Mock<IGroupParticipantRepository> _groupParticipantRepositoryMock = new();
    private readonly Mock<IValidator<CreateMemeRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateMemeRequest>> _updateValidatorMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly MemeService _memeService;

    public MemeServiceTests()
    {
        _currentUserMock.Setup(mock => mock.UserId).Returns(CurrentUserId);

        _createValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateMemeRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateMemeRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _memeService = new MemeService(
            _memeRepositoryMock.Object,
            _groupParticipantRepositoryMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _currentUserMock.Object);
    }

    #region GetAllForGroupAsync

    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsSucceededResultWithEmptyResultArray()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetAllByGroupIdAsyncToReturn(groupId, []);

        // Act
        var result = await _memeService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsSucceededResultWithApropriateItemsCount()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetAllByGroupIdAsyncToReturn(groupId, [GetMemeData(), GetMemeData(), GetMemeData()]);

        // Act
        var result = await _memeService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();

        // Act
        var result = await _memeService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _memeRepositoryMock.Verify(
            mock => mock.GetAllByGroupIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ForMemeId_ReturnsFailuredResult_WhenNoMemeWithGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid memeId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _memeService.GetByIdAsync(groupId, memeId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Be("No meme with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task GetByIdAsync_ForMemeId_ReturnsSucceededResultWithMemeResult()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Meme meme = GetMemeData(text: "meme text");
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, meme);

        // Act
        var result = await _memeService.GetByIdAsync(groupId, meme.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(meme.Id);
        result.Value.Text.Should().Be("meme text");
    }

    [Fact]
    public async Task GetByIdAsync_ForMemeId_ReturnsNotFoundResult_WhenMemeBelongsToAnotherGroup()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid otherGroupId = Guid.NewGuid();
        Meme meme = GetMemeData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(otherGroupId, meme);

        // Act
        var result = await _memeService.GetByIdAsync(groupId, meme.Id, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ForMemeId_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Meme meme = GetMemeData();
        SetupGetByIdAsyncToReturn(groupId, meme);

        // Act
        var result = await _memeService.GetByIdAsync(groupId, meme.Id, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _memeRepositoryMock.Verify(
            mock => mock.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ForCreateMemeRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        _createValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateMemeRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

        // Act
        var act = async () => await _memeService.AddAsync(groupId, GetCreateMemeRequest(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddAsync_ForCreateMemeRequest_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();

        // Act
        var result = await _memeService.AddAsync(groupId, GetCreateMemeRequest(), CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _memeRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Meme>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ForCreateMemeRequest_ReturnsMemeId_WhenMemeSuccessfullyCreated()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid createdMemeId = Guid.NewGuid();
        GroupParticipant participant = SetupCurrentUserAsMemberOf(groupId);
        _memeRepositoryMock
            .Setup(mock => mock.AddAsync(It.IsAny<Meme>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdMemeId);

        // Act
        var result = await _memeService.AddAsync(groupId, GetCreateMemeRequest(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(createdMemeId);
        _memeRepositoryMock.Verify(
            mock => mock.AddAsync(It.Is<Meme>(q => q.CreatedById == participant.Id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_ForCreateMemeRequest_LooksUpParticipantForCurrentUser()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        await _memeService.AddAsync(groupId, GetCreateMemeRequest(), CancellationToken.None);

        // Assert
        _groupParticipantRepositoryMock.Verify(
            mock => mock.GetByUserIdAndGroupIdAsync(CurrentUserId, groupId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ForUpdateMemeRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        _updateValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateMemeRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

        // Act
        var act = async () => await _memeService.UpdateAsync(Guid.NewGuid(), groupId, GetUpdateMemeRequest(),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateMemeRequest_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Meme meme = GetMemeData();
        SetupGetByIdAsyncToReturn(groupId, meme);

        // Act
        var result = await _memeService.UpdateAsync(meme.Id, groupId, GetUpdateMemeRequest(), CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _memeRepositoryMock.Verify(mock => mock.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ForMemeId_ReturnsFailuredResult_WhenNoMemeForGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid memeId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _memeService.UpdateAsync(memeId, groupId, GetUpdateMemeRequest(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("No meme with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateMemeRequest_ReturnsSucceededResult_WhenMemeSuccessfullyUpdated()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Meme meme = GetMemeData();
        GroupParticipant participant = SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, meme);

        // Act
        var result = await _memeService.UpdateAsync(meme.Id, groupId, GetUpdateMemeRequest("new text"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        meme.Text.Should().Be("new text");
        meme.LastModifiedById.Should().Be(participant.Id);
        _memeRepositoryMock.Verify(mock => mock.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsFailuredResult_WhenNoMemeWithGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid memeId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _memeService.DeleteAsync(groupId, memeId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("No meme with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsNotFoundResult_WhenMemeBelongsToAnotherGroup()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Meme meme = GetMemeData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(Guid.NewGuid(), meme);

        // Act
        var result = await _memeService.DeleteAsync(groupId, meme.Id, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _memeRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<Meme>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Meme meme = GetMemeData();
        SetupGetByIdAsyncToReturn(groupId, meme);

        // Act
        var result = await _memeService.DeleteAsync(groupId, meme.Id, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _memeRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<Meme>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsSuccessResult_WhenMemeSuccessfullyDeleted()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Meme meme = GetMemeData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, meme);

        // Act
        var result = await _memeService.DeleteAsync(groupId, meme.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _memeRepositoryMock.Verify(mock => mock.DeleteAsync(meme, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test_Factory_Methods

    /// <summary>
    /// Makes the current user a participant of <paramref name="groupId"/>. Without this call the user is not a member of any group.
    /// </summary>
    private GroupParticipant SetupCurrentUserAsMemberOf(Guid groupId)
    {
        var participant = GroupParticipant.Create(groupId, CurrentUserId);
        _groupParticipantRepositoryMock
            .Setup(mock => mock.GetByUserIdAndGroupIdAsync(CurrentUserId, groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(participant);

        return participant;
    }

    private void SetupGetAllByGroupIdAsyncToReturn(Guid groupId, Meme[] memes)
        => _memeRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memes);

    /// <summary>
    /// Stores <paramref name="meme"/> in <paramref name="groupId"/>; looking it up with any other group returns null.
    /// </summary>
    private void SetupGetByIdAsyncToReturn(Guid groupId, Meme meme)
        => _memeRepositoryMock
            .Setup(mock => mock.GetByIdAsync(groupId, meme.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(meme);

    private static void AssertGroupNotFound(Application.Common.Result result, Guid groupId)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Be($"No group with ID: {groupId} found");
    }

    private static CreateMemeRequest GetCreateMemeRequest(string text = "meme text")
        => new("https://server/memes/meme.webp", text);
    private static UpdateMemeRequest GetUpdateMemeRequest(string text = "meme text") => new(text);

    private static Meme GetMemeData(Guid? createdById = null, string text = "test text")
        => Meme.Create(createdById ?? Guid.NewGuid(), "https://server/memes/meme.webp", text);

    #endregion
}
