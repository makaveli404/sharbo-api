using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Quote;
using SharboAPI.Application.Services;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;

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
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsSucceededResultWithApropriateItemsCount()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Quote[] expectedQuoteResults = [
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
            .ReturnsAsync(expectedQuoteResults);

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

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        SetupGetByIdAsyncToReturnQuoteResult(quoteRepositoryMock, expectedQuoteResult);

        // Act
        var result = await quoteService.GetByIdAsync(quoteId, CancellationToken.None);

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

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        SetupGetByIdAsyncToReturnQuoteResult(quoteRepositoryMock, expectedQuoteResult);

        // Act
        var result = await quoteService.GetByIdAsync(quoteId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<QuoteResult?>();
        result.Value.Text.Should().Be(text);
    }

    [Fact]
    public async Task AddAsync_ForCreateQuoteRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange 
        CreateQuoteRequest request = GetCreateQuoteRequest();

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        validatorForCreateQuoteRequestMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateQuoteRequest>>(), CancellationToken.None))
            .ThrowsAsync(new ArgumentException());

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = async () => await quoteService.AddAsync(request, CancellationToken.None);

        // Assert
        await result
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddAsync_ForCreateQuoteRequest_ReturnsFailuredResult_WhenNoGroupParticipantForRequestingUserId()
    {
        // Arrange 
        CreateQuoteRequest request = GetCreateQuoteRequest();
        GroupParticipant? groupParticipantResult = null;
        string expectedFailureMessage = "No participant found";

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupCreateValidatorMockToReturnValidationResult(validatorForCreateQuoteRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }


    [Fact]
    public async Task AddAsync_ForCreateQuoteRequest_ReturnsQuoteId_WhenQuoteSuccessfullyCreated()
    {
        // Arrange 
        Guid createdQuoteId = Guid.NewGuid();
        CreateQuoteRequest request = GetCreateQuoteRequest();
        GroupParticipant groupParticipantResult = GetGroupParticipantData();

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupCreateValidatorMockToReturnValidationResult(validatorForCreateQuoteRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        quoteRepositoryMock
            .Setup(mock => mock.AddAsync(It.IsAny<Quote>(), CancellationToken.None))
            .ReturnsAsync(createdQuoteId);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(createdQuoteId);
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateQuoteRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange 
        Guid quoteId = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();
        UpdateQuoteRequest request = GetUpdateQuoteRequest();

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        validatorForUpdateQuoteRequestMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateQuoteRequest>>(), CancellationToken.None))
            .ThrowsAsync(new ArgumentException());

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = async () => await quoteService.UpdateAsync(quoteId, groupId, request, CancellationToken.None);

        // Assert
        await result
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateQuoteRequest_ReturnsFailuredResult_WhenNoGroupParticipantForRequestingUserId()
    {
        // Arrange 
        Guid quoteId = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();
        UpdateQuoteRequest request = GetUpdateQuoteRequest();
        GroupParticipant? groupParticipantResult = null;
        string expectedFailureMessage = "No participant found";

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupUpdateValidatorMockToReturnValidationResult(validatorForUpdateQuoteRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.UpdateAsync(quoteId, groupId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task UpdateAsync_ForQuoteId_ReturnsFailuredResult_WhenNoQuoteForGivenIdFound()
    {
        // Arrange 
        Guid quoteId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid groupId = Guid.NewGuid();
        UpdateQuoteRequest request = GetUpdateQuoteRequest();
        GroupParticipant groupParticipantResult = GetGroupParticipantData();
        Quote? quoteResult = null;
        string expectedFailureMessage = "No quote with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupUpdateValidatorMockToReturnValidationResult(validatorForUpdateQuoteRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);
        SetupGetByIdAsyncToReturnQuoteResult(quoteRepositoryMock, quoteResult);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.UpdateAsync(quoteId, groupId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateQuoteRequest_ReturnsSucceededResult_WhenQuoteSuccessfullyUpdated()
    {
        // Arrange 
        Guid quoteId = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();
        UpdateQuoteRequest request = GetUpdateQuoteRequest();
        GroupParticipant groupParticipantResult = GetGroupParticipantData();
        Quote quoteResult = GetQuoteData();

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupUpdateValidatorMockToReturnValidationResult(validatorForUpdateQuoteRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);
        SetupGetByIdAsyncToReturnQuoteResult(quoteRepositoryMock, quoteResult);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.UpdateAsync(quoteId, groupId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        quoteRepositoryMock
            .Verify(mock => mock.SaveChangesAsync(CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsFailuredResult_WhenNoQuoteWithGivenIdFound()
    {
        // Arrange
        Guid quoteId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Quote? quoteResult = null;
        string expectedFailureMessage = "No quote with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        quoteRepositoryMock
            .Setup(mock => mock.GetByIdAsync(quoteId, CancellationToken.None))
            .ReturnsAsync(quoteResult);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.DeleteAsync(quoteId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsSuccessResult_WhenQuoteSuccessfullyDeleted()
    {
        // Arrange
        Guid quoteId = Guid.NewGuid();
        Quote? quoteResult = GetQuoteData();

        var quoteRepositoryMock = CreateQuoteRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateQuoteRequestMock = CreateValidatorForCreateQuoteRequestMock();
        var validatorForUpdateQuoteRequestMock = CreateValidatorForUpdateQuoteRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        quoteRepositoryMock
            .Setup(mock => mock.GetByIdAsync(quoteId, CancellationToken.None))
            .ReturnsAsync(quoteResult);

        var quoteService = new QuoteService(
            quoteRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateQuoteRequestMock.Object,
            validatorForUpdateQuoteRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await quoteService.DeleteAsync(quoteId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        quoteRepositoryMock
            .Verify(mock => mock.DeleteAsync(quoteResult, CancellationToken.None), Times.Once());
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
