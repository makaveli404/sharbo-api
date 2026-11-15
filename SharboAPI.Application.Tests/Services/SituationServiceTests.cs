using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Situation;
using SharboAPI.Application.Services;
using FluentValidation;
using FluentValidation.Results;

namespace SharboAPI.Application.Tests.Services;

// xUnit creates a new instance per test, so every test gets fresh mocks.
public class SituationServiceTests
{
    private const string CurrentUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

    private readonly Mock<ISituationRepository> _situationRepositoryMock = new();
    private readonly Mock<IGroupParticipantRepository> _groupParticipantRepositoryMock = new();
    private readonly Mock<IValidator<CreateSituationRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateSituationRequest>> _updateValidatorMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly SituationService _situationService;

    public SituationServiceTests()
    {
        _currentUserMock.Setup(mock => mock.UserId).Returns(CurrentUserId);

        _createValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateSituationRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateSituationRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _situationService = new SituationService(
            _situationRepositoryMock.Object,
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
        var result = await _situationService.GetAllForGroupAsync(groupId, CancellationToken.None);

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
        SetupGetAllByGroupIdAsyncToReturn(groupId, [GetSituationData(), GetSituationData(), GetSituationData()]);

        // Act
        var result = await _situationService.GetAllForGroupAsync(groupId, CancellationToken.None);

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
        var result = await _situationService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _situationRepositoryMock.Verify(
            mock => mock.GetAllByGroupIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ForSituationId_ReturnsFailuredResult_WhenNoSituationWithGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid situationId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _situationService.GetByIdAsync(groupId, situationId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Be("No situation with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task GetByIdAsync_ForSituationId_ReturnsSucceededResultWithSituationResult()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation situation = GetSituationData(text: "situation text");
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, situation);

        // Act
        var result = await _situationService.GetByIdAsync(groupId, situation.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(situation.Id);
        result.Value.Text.Should().Be("situation text");
    }

    [Fact]
    public async Task GetByIdAsync_ForSituationId_ReturnsNotFoundResult_WhenSituationBelongsToAnotherGroup()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid otherGroupId = Guid.NewGuid();
        Situation situation = GetSituationData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(otherGroupId, situation);

        // Act
        var result = await _situationService.GetByIdAsync(groupId, situation.Id, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ForSituationId_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation situation = GetSituationData();
        SetupGetByIdAsyncToReturn(groupId, situation);

        // Act
        var result = await _situationService.GetByIdAsync(groupId, situation.Id, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _situationRepositoryMock.Verify(
            mock => mock.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ForCreateSituationRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        _createValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateSituationRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

        // Act
        var act = async () => await _situationService.AddAsync(groupId, GetCreateSituationRequest(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddAsync_ForCreateSituationRequest_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();

        // Act
        var result = await _situationService.AddAsync(groupId, GetCreateSituationRequest(), CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _situationRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Situation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ForCreateSituationRequest_ReturnsSituationId_WhenSituationSuccessfullyCreated()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid createdSituationId = Guid.NewGuid();
        GroupParticipant participant = SetupCurrentUserAsMemberOf(groupId);
        _situationRepositoryMock
            .Setup(mock => mock.AddAsync(It.IsAny<Situation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdSituationId);

        // Act
        var result = await _situationService.AddAsync(groupId, GetCreateSituationRequest(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(createdSituationId);
        _situationRepositoryMock.Verify(
            mock => mock.AddAsync(It.Is<Situation>(q => q.CreatedById == participant.Id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_ForCreateSituationRequest_LooksUpParticipantForCurrentUser()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        await _situationService.AddAsync(groupId, GetCreateSituationRequest(), CancellationToken.None);

        // Assert
        _groupParticipantRepositoryMock.Verify(
            mock => mock.GetByUserIdAndGroupIdAsync(CurrentUserId, groupId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ForUpdateSituationRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        _updateValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateSituationRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

        // Act
        var act = async () => await _situationService.UpdateAsync(Guid.NewGuid(), groupId, GetUpdateSituationRequest(),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateSituationRequest_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation situation = GetSituationData();
        SetupGetByIdAsyncToReturn(groupId, situation);

        // Act
        var result = await _situationService.UpdateAsync(situation.Id, groupId, GetUpdateSituationRequest(), CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _situationRepositoryMock.Verify(mock => mock.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ForSituationId_ReturnsFailuredResult_WhenNoSituationForGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid situationId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _situationService.UpdateAsync(situationId, groupId, GetUpdateSituationRequest(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("No situation with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateSituationRequest_ReturnsSucceededResult_WhenSituationSuccessfullyUpdated()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation situation = GetSituationData();
        GroupParticipant participant = SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, situation);

        // Act
        var result = await _situationService.UpdateAsync(situation.Id, groupId, GetUpdateSituationRequest("new text"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        situation.Text.Should().Be("new text");
        situation.LastModifiedById.Should().Be(participant.Id);
        _situationRepositoryMock.Verify(mock => mock.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsFailuredResult_WhenNoSituationWithGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid situationId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _situationService.DeleteAsync(groupId, situationId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("No situation with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsNotFoundResult_WhenSituationBelongsToAnotherGroup()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation situation = GetSituationData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(Guid.NewGuid(), situation);

        // Act
        var result = await _situationService.DeleteAsync(groupId, situation.Id, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _situationRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<Situation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation situation = GetSituationData();
        SetupGetByIdAsyncToReturn(groupId, situation);

        // Act
        var result = await _situationService.DeleteAsync(groupId, situation.Id, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _situationRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<Situation>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsSuccessResult_WhenSituationSuccessfullyDeleted()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation situation = GetSituationData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, situation);

        // Act
        var result = await _situationService.DeleteAsync(groupId, situation.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _situationRepositoryMock.Verify(mock => mock.DeleteAsync(situation, It.IsAny<CancellationToken>()), Times.Once);
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

    private void SetupGetAllByGroupIdAsyncToReturn(Guid groupId, Situation[] situations)
        => _situationRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(situations);

    /// <summary>
    /// Stores <paramref name="situation"/> in <paramref name="groupId"/>; looking it up with any other group returns null.
    /// </summary>
    private void SetupGetByIdAsyncToReturn(Guid groupId, Situation situation)
        => _situationRepositoryMock
            .Setup(mock => mock.GetByIdAsync(groupId, situation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(situation);

    private static void AssertGroupNotFound(Application.Common.Result result, Guid groupId)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Be($"No group with ID: {groupId} found");
    }

    private static CreateSituationRequest GetCreateSituationRequest(string text = "situation text") => new(text);
    private static UpdateSituationRequest GetUpdateSituationRequest(string text = "situation text") => new(text);

    private static Situation GetSituationData(Guid? createdById = null, string text = "test text")
        => Situation.Create(createdById ?? Guid.NewGuid(), text);

    #endregion
}
