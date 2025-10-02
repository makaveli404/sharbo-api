using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Meme;
using SharboAPI.Application.Services;
using Microsoft.AspNetCore.Http;
using FluentValidation;
using FluentValidation.Results;

namespace SharboAPI.Application.Tests.Services;

public class MemeServiceTests
{
    [Fact]
    public async Task GetAllForGroupAsync_ForGroupId_ReturnsSucceededResultWithApropriateItemsCount()
    {
        // Arrange 
        Guid groupId = Guid.NewGuid();
        Meme[] expectedMemeResults = [
            GetMemeData(),
            GetMemeData(),
            GetMemeData(),
        ];

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        memeRepositoryMock
            .Setup(mock => mock.GetAllByGroupIdAsync(groupId, CancellationToken.None))
            .ReturnsAsync(expectedMemeResults);

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await memeService.GetAllForGroupAsync(groupId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value
            .Count()
            .Should()
            .Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_ForMemeId_ReturnsFailuredResult_WhenNoMemeWithGivenIdFound()
    {
        // Arrange 
        Guid memeId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Meme? memeResult = null;
        string expectedFailureDescription = "No meme with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        memeRepositoryMock
            .Setup(mock => mock.GetByIdAsync(memeId, CancellationToken.None))
            .ReturnsAsync(memeResult);

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await memeService.GetByIdAsync(memeId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureDescription);
    }

    [Fact]
    public async void GetByIdAsync_ForMemeId_ReturnsSucceededResultWithMemeResult()
    {
        // Arrange 
        Guid memeId = Guid.NewGuid();
        Guid createdById = Guid.NewGuid();
        string imagePath = "https://myserver/imagePaths/meme32.jpg";
        string text = "Description for meme32.jpg";
        
        Meme expectedMemeResult = GetMemeData(createdById, imagePath, text); 

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        memeRepositoryMock
            .Setup(mock => mock.GetByIdAsync(memeId, CancellationToken.None))
            .ReturnsAsync(expectedMemeResult);

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await memeService.GetByIdAsync(memeId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<MemeResult?>();

        result.Value.CreatedById.Should().Be(createdById);
        result.Value.ImagePath.Should().Be(imagePath);
        result.Value.Text.Should().Be(text);
    }

    [Fact]  
    public async Task AddAsync_ForCreateMemeRequest_ThrowsArgumentException_WhenValidationFailed()
    {
        // Arrange 
        CreateMemeRequest request = GetCreateMemeRequest();

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        validatorForCreateMemeRequestMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<ValidationContext<CreateMemeRequest>>(), CancellationToken.None))
            .ThrowsAsync(new ArgumentException());

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = async () => await memeService.AddAsync(request, CancellationToken.None);

        // Assert
        await result
            .Should()
            .ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task AddAsync_ForCreateMemeRequest_ReturnsFailuredResult_WhenNoGroupParticipantForRequestingUserId()
    {
        // Arrange 
        CreateMemeRequest request = GetCreateMemeRequest();
        GroupParticipant? groupParticipantResult = null;
        string expectedFailureMessage = "No participant found";

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupCreateValidatorMockToReturnValidationResult(validatorForCreateMemeRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await memeService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task AddAsync_ForCreateMemeRequest_ReturnsMemeId_WhenMemeSuccessfullyCreated()
    {
        // Arrange 
        Guid createdMemeId = Guid.NewGuid();
        CreateMemeRequest request = GetCreateMemeRequest();
        GroupParticipant groupParticipantResult = GetGroupParticipantData();

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        SetupCreateValidatorMockToReturnValidationResult(validatorForCreateMemeRequestMock);
        SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(groupParticipantRepositoryMock, groupParticipantResult);

        memeRepositoryMock
            .Setup(mock => mock.AddAsync(It.IsAny<Meme>(), CancellationToken.None))
            .ReturnsAsync(createdMemeId);

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await memeService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(createdMemeId);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsFailuredResult_WhenNoMemeWithGivenIdFound()
    {
        // Arrange
        Guid memeId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Meme? memeResult = null;
        string expectedFailureMessage = "No meme with ID: 27c82825-1d93-4210-8a63-d43e3c7c46d4 found";

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        memeRepositoryMock
            .Setup(mock => mock.GetByIdAsync(memeId, CancellationToken.None))
            .ReturnsAsync(memeResult);

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await memeService.DeleteAsync(memeId, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Be(expectedFailureMessage);
    }

    [Fact]
    public async Task DeleteAsync_ForGivenId_ReturnsSuccessResult_WhenMemeSuccessfullyDeleted()
    {
        // Arrange
        Guid memeId = Guid.NewGuid();
        Meme? memeResult = GetMemeData();

        var memeRepositoryMock = CreateMemeRepositoryMock();
        var groupParticipantRepositoryMock = CreateGroupParticipantRepositoryMock();
        var validatorForCreateMemeRequestMock = CreateValidatorForCreateMemeRequestMock();
        var validatorForUpdateMemeRequestMock = CreateValidatorForUpdateMemeRequestMock();
        var httpContextAccessorMock = CreateHttpContextAccessorMock();

        memeRepositoryMock
            .Setup(mock => mock.GetByIdAsync(memeId, CancellationToken.None))
            .ReturnsAsync(memeResult);

        var memeService = CreateMemeService(
            memeRepositoryMock.Object,
            groupParticipantRepositoryMock.Object,
            validatorForCreateMemeRequestMock.Object,
            validatorForUpdateMemeRequestMock.Object,
            httpContextAccessorMock.Object);

        // Act
        var result = await memeService.DeleteAsync(memeId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        memeRepositoryMock
            .Verify(mock => mock.DeleteAsync(memeResult, CancellationToken.None), Times.Once());
    }

    #region Test_Factory_Methods

    private static Mock<IMemeRepository> CreateMemeRepositoryMock() => new();
    private static Mock<IGroupParticipantRepository> CreateGroupParticipantRepositoryMock() => new();
    private static Mock<IValidator<CreateMemeRequest>> CreateValidatorForCreateMemeRequestMock() => new();
    private static Mock<IValidator<UpdateMemeRequest>> CreateValidatorForUpdateMemeRequestMock() => new();
    private static Mock<IHttpContextAccessor> CreateHttpContextAccessorMock() => new();
    private static MemeService CreateMemeService(IMemeRepository memeRepository,
                                                 IGroupParticipantRepository groupParticipantRepository,
                                                 IValidator<CreateMemeRequest> createMemeRequestValidator,
                                                 IValidator<UpdateMemeRequest> updateMemeRequestValidator,
                                                 IHttpContextAccessor httpContextAccessor) 
        => new(memeRepository, groupParticipantRepository, createMemeRequestValidator, updateMemeRequestValidator, httpContextAccessor);

    private static void SetupCreateValidatorMockToReturnValidationResult(Mock<IValidator<CreateMemeRequest>> createValidatorMock,
                                                                         ValidationResult? result = null)
        => createValidatorMock
            .Setup(mock => mock.ValidateAsync(It.IsAny<CreateMemeRequest>(), CancellationToken.None))
            .ReturnsAsync(result ?? new ValidationResult());

    private static void SetupGetByUserIdAndGroupIdAsyncToReturnGroupParticipantResult(
        Mock<IGroupParticipantRepository> groupParticipantRepositoryMock,
        GroupParticipant? groupParticipantResult = null)
            => groupParticipantRepositoryMock
                .Setup(mock => mock.GetByUserIdAndGroupIdAsync(It.IsAny<string>(), It.IsAny<Guid>(), CancellationToken.None))
                .ReturnsAsync(groupParticipantResult);

    private static CreateMemeRequest GetCreateMemeRequest(Guid? groupId = null,
                                                          string imagePath = "https://myserver/imagePaths/meme32.jpg",
                                                          string text = "meme32 description")
        => new(groupId ?? Guid.NewGuid(), imagePath, text);

    private static Meme GetMemeData(Guid? CreatedById = null, string imagePath = "https://server/paths/", string? text = null)
        => Meme.Create(CreatedById ?? Guid.NewGuid(), imagePath, text);

    private static GroupParticipant GetGroupParticipantData(string userId = "AJNQPMbMtHNRHuXLDVs19Lt5J1A2",
                                                            List<GroupParticipantRole>? roles = null)
        => GroupParticipant.Create(userId, roles ?? []);

    #endregion
}
