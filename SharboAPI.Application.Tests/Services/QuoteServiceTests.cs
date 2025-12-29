using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Quote;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;
using SharboAPI.Application.Services;
using SharboAPI.Application.DTO.Situation;

namespace SharboAPI.Application.Tests.Services;

public class QuoteServiceTests
{
    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsSucceededResultWithEmptyResultArray()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote[] expectedQuoteResults = [];

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        quoteRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, CancellationToken.None))
            .ReturnsAsync(expectedQuoteResults);

        var situationService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await situationService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsSucceededResultWithApropriateItemsCount()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote[] expectedSituationResults = [
            GetQuoteData(),
            GetQuoteData(),
            GetQuoteData()
        ];

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        quoteRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, CancellationToken.None))
            .ReturnsAsync(expectedSituationResults);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value
            .Count()
            .Should()
            .Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_ForQuoteId_ReturnsFailuredResult_WhenNoQuoteWithGivenIdFound()
    {
        // Arrange
        Guid quoteId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Quote? expectedQuoteResult = null;
        string expectedErrorMessage = "No quote with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        var situationService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        SetupGetByIdAsyncToReturnQuoteResult(quoteRepositoryMock, expectedQuoteResult);

        // Act
        var result = await situationService.GetByIdAsync(quoteId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedErrorMessage);
    }


    [Fact]
    public async void GetByIdAsync_ForQuoteId_ReturnsSucceededResultWithQuoteResult()
    {
        // Arrange
        Guid quoteId = Guid.NewGuid();
        string text = "quote text";
        Quote expectedQuoteResult = GetQuoteData(text: text);

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        var situationService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        SetupGetByIdAsyncToReturnQuoteResult(quoteRepositoryMock, expectedQuoteResult);

        // Act
        var result = await situationService.GetByIdAsync(quoteId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<QuoteResult?>();
        result.Value.Text.Should().Be(text);
    }

    #region Test_Factory_Methods

    private static Mock<IValidator<CreateQuoteRequest>> CreateValidatorForCreateQuoteRequestMock() => new();
    private static Mock<IValidator<UpdateQuoteRequest>> CreateValidatorForUpdateQuoteRequestMock() => new();
    private static Mock<IQuoteRepository> CreateQuoteRepositoryMock() => new();
    private static Mock<IGroupParticipantRepository> CreateGroupParticipantRepositoryMock() => new();
    private static Mock<IHttpContextAccessor> CreateHttpContextAccessorMock() => new();

    private static void SetupGetByIdAsyncToReturnQuoteResult(Mock<IQuoteRepository> mock,
                                                             Quote? expectedQuoteResult)
        => mock
            .Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(expectedQuoteResult);

    private static void SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(Mock<IGroupParticipantRepository> mock,
                                                                                      GroupParticipant? groupParticipantResult)
        => mock
            .Setup(mock => mock.GetByUserIdAndGroupIdAsync(It.IsAny<string>(), It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(groupParticipantResult);

    private void SetupCreateValidatorMockToReturnValidationResult(Mock<IValidator<CreateQuoteRequest>> mock,
                                                                  ValidationResult? result = null)
        => mock
            .Setup(mock => mock.ValidateAsync(It.IsAny<CreateQuoteRequest>(), CancellationToken.None))
            .ReturnsAsync(result ?? new ValidationResult());

    private static void SetupUpdateValidatorMockToReturnValidationResult(Mock<IValidator<UpdateQuoteRequest>> mock,
                                                                         ValidationResult? result = null)
        => mock
            .Setup(mock => mock.ValidateAsync(It.IsAny<UpdateQuoteRequest>(), CancellationToken.None))
            .ReturnsAsync(result ?? new ValidationResult());

    private static CreateQuoteRequest GetCreateQuoteRequest(string text = "quote text") => new(Guid.NewGuid(), text);
    private static UpdateQuoteRequest GetUpdateQuoteRequest(string text = "quote text") => new(text);

    private static Quote GetQuoteData(Guid? CreatedById = null, string text = "test text")
        => Quote.Create(CreatedById ?? Guid.NewGuid(), text);

    private static GroupParticipant GetGroupParticipantData(string userId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2",
                                                            List<GroupParticipantRole>? roles = null)
        => GroupParticipant.Create(userId, roles ?? []);

    #endregion
}
