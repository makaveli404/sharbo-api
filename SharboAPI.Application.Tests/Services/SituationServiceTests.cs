using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Situation;
using SharboAPI.Application.Services;
using Microsoft.AspNetCore.Http;
using FluentValidation;

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

    private static Situation GetSituationData(Guid? CreatedById = null, string text = "test text")
        => Situation.Create(CreatedById ?? Guid.NewGuid(), text);

    #endregion
}
