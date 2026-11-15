using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Quote;
using SharboAPI.Application.Services;
using FluentValidation;
using FluentValidation.Results;

namespace SharboAPI.Application.Tests.Services;

// xUnit creates a new instance per test, so every test gets fresh mocks.
public class QuoteServiceTests
{
    private const string CurrentUserId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2";

    private readonly Mock<IQuoteRepository> _quoteRepositoryMock = new();
    private readonly Mock<IGroupParticipantRepository> _groupParticipantRepositoryMock = new();
    private readonly Mock<IValidator<CreateQuoteRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<UpdateQuoteRequest>> _updateValidatorMock = new();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly QuoteService _quoteService;

    public QuoteServiceTests()
    {
        _currentUserMock.Setup(mock => mock.UserId).Returns(CurrentUserId);

        _createValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateQuoteRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateQuoteRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _quoteService = new QuoteService(
            _quoteRepositoryMock.Object,
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
        var result = await _quoteService.GetAllForGroupAsync(groupId, CancellationToken.None);

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
        SetupGetAllByGroupIdAsyncToReturn(groupId, [GetQuoteData(), GetQuoteData(), GetQuoteData()]);

        // Act
        var result = await _quoteService.GetAllForGroupAsync(groupId, CancellationToken.None);

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
        var result = await _quoteService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _quoteRepositoryMock.Verify(
            mock => mock.GetAllByGroupIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ForQuoteId_ReturnsFailuredResult_WhenNoQuoteWithGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid quoteId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _quoteService.GetByIdAsync(groupId, quoteId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Be("No quote with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task GetByIdAsync_ForQuoteId_ReturnsSucceededResultWithQuoteResult()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote quote = GetQuoteData(text: "quote text");
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, quote);

        // Act
        var result = await _quoteService.GetByIdAsync(groupId, quote.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(quote.Id);
        result.Value.Text.Should().Be("quote text");
    }

    [Fact]
    public async Task GetByIdAsync_ForQuoteId_ReturnsNotFoundResult_WhenQuoteBelongsToAnotherGroup()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid otherGroupId = Guid.NewGuid();
        Quote quote = GetQuoteData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(otherGroupId, quote);

        // Act
        var result = await _quoteService.GetByIdAsync(groupId, quote.Id, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ForQuoteId_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote quote = GetQuoteData();
        SetupGetByIdAsyncToReturn(groupId, quote);

        // Act
        var result = await _quoteService.GetByIdAsync(groupId, quote.Id, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _quoteRepositoryMock.Verify(
            mock => mock.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region AddAsync

    [Fact]
    public async Task AddAsync_ForCreateQuoteRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        _createValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateQuoteRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

        // Act
        var act = async () => await _quoteService.AddAsync(groupId, GetCreateQuoteRequest(), CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddAsync_ForCreateQuoteRequest_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();

        // Act
        var result = await _quoteService.AddAsync(groupId, GetCreateQuoteRequest(), CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _quoteRepositoryMock.Verify(mock => mock.AddAsync(It.IsAny<Quote>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddAsync_ForCreateQuoteRequest_ReturnsQuoteId_WhenQuoteSuccessfullyCreated()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid createdQuoteId = Guid.NewGuid();
        GroupParticipant participant = SetupCurrentUserAsMemberOf(groupId);
        _quoteRepositoryMock
            .Setup(mock => mock.AddAsync(It.IsAny<Quote>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdQuoteId);

        // Act
        var result = await _quoteService.AddAsync(groupId, GetCreateQuoteRequest(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(createdQuoteId);
        _quoteRepositoryMock.Verify(
            mock => mock.AddAsync(It.Is<Quote>(q => q.CreatedById == participant.Id), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_ForCreateQuoteRequest_LooksUpParticipantForCurrentUser()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        await _quoteService.AddAsync(groupId, GetCreateQuoteRequest(), CancellationToken.None);

        // Assert
        _groupParticipantRepositoryMock.Verify(
            mock => mock.GetByUserIdAndGroupIdAsync(CurrentUserId, groupId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ForUpdateQuoteRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        SetupCurrentUserAsMemberOf(groupId);
        _updateValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateQuoteRequest>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException());

        // Act
        var act = async () => await _quoteService.UpdateAsync(Guid.NewGuid(), groupId, GetUpdateQuoteRequest(),
            CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateQuoteRequest_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote quote = GetQuoteData();
        SetupGetByIdAsyncToReturn(groupId, quote);

        // Act
        var result = await _quoteService.UpdateAsync(quote.Id, groupId, GetUpdateQuoteRequest(), CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _quoteRepositoryMock.Verify(mock => mock.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ForQuoteId_ReturnsFailuredResult_WhenNoQuoteForGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid quoteId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _quoteService.UpdateAsync(quoteId, groupId, GetUpdateQuoteRequest(), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("No quote with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateQuoteRequest_ReturnsSucceededResult_WhenQuoteSuccessfullyUpdated()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote quote = GetQuoteData();
        GroupParticipant participant = SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, quote);

        // Act
        var result = await _quoteService.UpdateAsync(quote.Id, groupId, GetUpdateQuoteRequest("new text"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        quote.Text.Should().Be("new text");
        quote.LastModifiedById.Should().Be(participant.Id);
        _quoteRepositoryMock.Verify(mock => mock.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsFailuredResult_WhenNoQuoteWithGivenIdFound()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Guid quoteId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        SetupCurrentUserAsMemberOf(groupId);

        // Act
        var result = await _quoteService.DeleteAsync(groupId, quoteId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be("No quote with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found");
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsNotFoundResult_WhenQuoteBelongsToAnotherGroup()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote quote = GetQuoteData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(Guid.NewGuid(), quote);

        // Act
        var result = await _quoteService.DeleteAsync(groupId, quote.Id, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        _quoteRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<Quote>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsNotFoundResult_WhenUserIsNotGroupMember()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote quote = GetQuoteData();
        SetupGetByIdAsyncToReturn(groupId, quote);

        // Act
        var result = await _quoteService.DeleteAsync(groupId, quote.Id, CancellationToken.None);

        // Assert
        AssertGroupNotFound(result, groupId);
        _quoteRepositoryMock.Verify(mock => mock.DeleteAsync(It.IsAny<Quote>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsSuccessResult_WhenQuoteSuccessfullyDeleted()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote quote = GetQuoteData();
        SetupCurrentUserAsMemberOf(groupId);
        SetupGetByIdAsyncToReturn(groupId, quote);

        // Act
        var result = await _quoteService.DeleteAsync(groupId, quote.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _quoteRepositoryMock.Verify(mock => mock.DeleteAsync(quote, It.IsAny<CancellationToken>()), Times.Once);
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

    private void SetupGetAllByGroupIdAsyncToReturn(Guid groupId, Quote[] quotes)
        => _quoteRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quotes);

    /// <summary>
    /// Stores <paramref name="quote"/> in <paramref name="groupId"/>; looking it up with any other group returns null.
    /// </summary>
    private void SetupGetByIdAsyncToReturn(Guid groupId, Quote quote)
        => _quoteRepositoryMock
            .Setup(mock => mock.GetByIdAsync(groupId, quote.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(quote);

    private static void AssertGroupNotFound(Application.Common.Result result, Guid groupId)
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        result.Error.Message.Should().Be($"No group with ID: {groupId} found");
    }

    private static CreateQuoteRequest GetCreateQuoteRequest(string text = "quote text") => new(text);
    private static UpdateQuoteRequest GetUpdateQuoteRequest(string text = "quote text") => new(text);

    private static Quote GetQuoteData(Guid? createdById = null, string text = "test text")
        => Quote.Create(createdById ?? Guid.NewGuid(), text);

    #endregion
}
