using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Situation;
using SharboAPI.Application.Services;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;

namespace SharboAPI.Application.Tests.Services;

public class SituationServiceTests
{
    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsSucceededResultWithEmptyResultArray()
    {
        // Arrange
        Guid groupId = Guid.NewGuid();
        Situation[] expectedSituationResults = [];

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        situationRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, CancellationToken.None))
            .ReturnsAsync(expectedSituationResults);

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
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
        Situation[] expectedSituationResults = [
            GetSituationData(),    
            GetSituationData(),    
            GetSituationData()    
        ];

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        situationRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, CancellationToken.None))
            .ReturnsAsync(expectedSituationResults);

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await situationService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value
            .Count()
            .Should()
            .Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_ForSituationId_ReturnsFailuredResult_WhenNoSituationWithGivenIdFound()
    {
        // Arrange
        Guid situationId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Situation? expectedSituationResult = null;
        string expectedErrorMessage = "No situation with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        SetupGetByIdAsyncToReturnSituationResult(situationRepositoryMock, expectedSituationResult);

        // Act
        var result = await situationService.GetByIdAsync(situationId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedErrorMessage);
    }

    [Fact]
    public async void GetByIdAsync_ForSituationId_ReturnsSucceededResultWithSituationResult()
    {
        // Arrange
        Guid situationId = Guid.NewGuid();
        string text = "situation text";
        Situation expectedSituationResult = GetSituationData(text: text);

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        SetupGetByIdAsyncToReturnSituationResult(situationRepositoryMock, expectedSituationResult);

        // Act
        var result = await situationService.GetByIdAsync(situationId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<SituationResult?>();
        result.Value.Text.Should().Be(text);
    }

    [Fact]
    public async Task AddAsync_ForCreateSituationRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange 
        CreateSituationRequest request = GetCreateSituationRequest();

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        validatorForCreateSituationRequestMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateSituationRequest>>(), CancellationToken.None))
            .ThrowsAsync(new ArgumentException());

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = async () => await situationService.AddAsync(request, CancellationToken.None);

        // Assert
        await result
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddAsync_ForCreateSituationRequest_ReturnsFailuredResult_WhenNoGroupParticipantForRequestingUserId()
    {
        // Arrange 
        CreateSituationRequest request = GetCreateSituationRequest();
        GroupParticipant? groupParticipantResult = null;
        string expectedFailureMessage = "No participant found";

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupCreateValidatorMockToReturnValidationResult(validatorForCreateSituationRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await situationService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task AddAsync_ForCreateSituationRequest_ReturnsSituationId_WhenSituationSuccessfullyCreated()
    {
        // Arrange 
        Guid createdSituationId = Guid.NewGuid();
        CreateSituationRequest request = GetCreateSituationRequest();
        GroupParticipant groupParticipantResult = GetGroupParticipantData();

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupCreateValidatorMockToReturnValidationResult(validatorForCreateSituationRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        situationRepositoryMock
            .Setup(mock => mock.AddAsync(It.IsAny<Situation>(), CancellationToken.None))
            .ReturnsAsync(createdSituationId);

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await situationService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(createdSituationId);
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateSituationRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange 
        Guid situationId = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();
        UpdateSituationRequest request = GetUpdateSituationRequest();

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        validatorForUpdateSituationRequestMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<UpdateSituationRequest>>(), CancellationToken.None))
            .ThrowsAsync(new ArgumentException());

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = async () => await situationService.UpdateAsync(situationId, groupId, request, CancellationToken.None);

        // Assert
        await result
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateSituationRequest_ReturnsFailuredResult_WhenNoGroupParticipantForRequestingUserId()
    {
        // Arrange 
        Guid situationId = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();
        UpdateSituationRequest request = GetUpdateSituationRequest();
        GroupParticipant? groupParticipantResult = null;
        string expectedFailureMessage = "No participant found";

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupUpdateValidatorMockToReturnValidationResult(validatorForUpdateSituationRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await situationService.UpdateAsync(situationId, groupId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task UpdateAsync_ForSituationId_ReturnsFailuredResult_WhenNoSituationForGivenIdFound()
    {
        // Arrange 
        Guid situationId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid groupId = Guid.NewGuid();
        UpdateSituationRequest request = GetUpdateSituationRequest();
        GroupParticipant groupParticipantResult = GetGroupParticipantData();
        Situation? situationResult = null;
        string expectedFailureMessage = "No situation with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupUpdateValidatorMockToReturnValidationResult(validatorForUpdateSituationRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);
        SetupGetByIdAsyncToReturnSituationResult(situationRepositoryMock, situationResult);

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await situationService.UpdateAsync(situationId, groupId, request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task UpdateAsync_ForUpdateSituationRequest_ReturnsSucceededResult_WhenSituationSuccessfullyUpdated()
    {
        // Arrange 
        Guid situationId = Guid.NewGuid();
        Guid groupId = Guid.NewGuid();
        UpdateSituationRequest request = GetUpdateSituationRequest();
        GroupParticipant groupParticipantResult = GetGroupParticipantData();
        Situation situationResult = GetSituationData();

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupUpdateValidatorMockToReturnValidationResult(validatorForUpdateSituationRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);
        SetupGetByIdAsyncToReturnSituationResult(situationRepositoryMock, situationResult);

        var situationService = new SituationService(
            situationRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateSituationRequestMock.Object,
            validatorForUpdateSituationRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await situationService.UpdateAsync(situationId, groupId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        situationRepositoryMock
            .Verify(mock => mock.SaveChangesAsync(CancellationToken.None), Times.Once());
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsFailuredResult_WhenNoSituationWithGivenIdFound()
    {
        // Arrange
        Guid situationId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Situation? situationResult = null;
        string expectedFailureMessage = "No situation with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        situationRepositoryMock
            .Setup(mock => mock.GetByIdAsync(situationId, CancellationToken.None))
            .ReturnsAsync(situationResult);

        var situationService = new SituationService(
           situationRepositoryMock.Object,
           groupParticipantRepositoryMock.Object,
           validatorForCreateSituationRequestMock.Object,
           validatorForUpdateSituationRequestMock.Object,
           httpContextAccessorMock.Object);

        // Act
        var result = await situationService.DeleteAsync(situationId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsSuccessResult_WhenSituationSuccessfullyDeleted()
    {
        // Arrange
        Guid situationId = Guid.NewGuid();
        Situation? situationResult = GetSituationData();

        var situationRepositoryMock = CreateSituationRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateSituationRequestMock = CreateValidatorForCreateSituationRequestMock();
        var validatorForUpdateSituationRequestMock = CreateValidatorForUpdateSituationRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        situationRepositoryMock
            .Setup(mock => mock.GetByIdAsync(situationId, CancellationToken.None))
            .ReturnsAsync(situationResult);

        var situationService = new SituationService(
           situationRepositoryMock.Object,
           groupParticipantRepositoryMock.Object,
           validatorForCreateSituationRequestMock.Object,
           validatorForUpdateSituationRequestMock.Object,
           httpContextAccessorMock.Object);

        // Act
        var result = await situationService.DeleteAsync(situationId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        situationRepositoryMock
            .Verify(mock => mock.DeleteAsync(situationResult, CancellationToken.None), Times.Once());
    }

    #region Test_Factory_Methods

    private static Mock<IValidator<CreateSituationRequest>> CreateValidatorForCreateSituationRequestMock() => new();
    private static Mock<IValidator<UpdateSituationRequest>> CreateValidatorForUpdateSituationRequestMock() => new();
    private static Mock<ISituationRepository> CreateSituationRepositoryMock() => new();
    private static Mock<IGroupParticipantRepository> CreateGroupParticipantRepositoryMock() => new();
    private static Mock<IHttpContextAccessor> CreateHttpContextAccessorMock() => new();

    private static void SetupGetByIdAsyncToReturnSituationResult(Mock<ISituationRepository> mock,
                                                                 Situation? expectedSituationResult)
        => mock
            .Setup(mock => mock.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(expectedSituationResult);

    private static void SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(Mock<IGroupParticipantRepository> mock,
                                                                                      GroupParticipant? groupParticipantResult)
        => mock
            .Setup(mock => mock.GetByUserIdAndGroupIdAsync(It.IsAny<string>(), It.IsAny<Guid>(), CancellationToken.None))
            .ReturnsAsync(groupParticipantResult);

    private void SetupCreateValidatorMockToReturnValidationResult(Mock<IValidator<CreateSituationRequest>> mock,
                                                                  ValidationResult? result = null)
        => mock
            .Setup(mock => mock.ValidateAsync(It.IsAny<CreateSituationRequest>(), CancellationToken.None))
            .ReturnsAsync(result ?? new ValidationResult());

    private static void SetupUpdateValidatorMockToReturnValidationResult(Mock<IValidator<UpdateSituationRequest>> mock,
                                                                     ValidationResult? result = null)
        => mock
            .Setup(mock => mock.ValidateAsync(It.IsAny<UpdateSituationRequest>(), CancellationToken.None))
            .ReturnsAsync(result ?? new ValidationResult());

    private static CreateSituationRequest GetCreateSituationRequest(string text = "situation text") => new(Guid.NewGuid(), text);
    private static UpdateSituationRequest GetUpdateSituationRequest(string text = "situation text") => new(text);

    private static Situation GetSituationData(Guid? CreatedById = null, string text = "test text")
        => Situation.Create(CreatedById ?? Guid.NewGuid(), text);

    private static GroupParticipant GetGroupParticipantData(string userId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2",
                                                            List<GroupParticipantRole>? roles = null)
        => GroupParticipant.Create(userId, roles ?? []);

    #endregion
}
