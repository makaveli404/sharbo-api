using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Meme;
using SharboAPI.Application.Services;
using Microsoft.AspNetCore.Http;
using FluentValidation;

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

    private static Meme GetMemeData(Guid? CreatedById = null, string imagePath = "https://server/paths/", string? text = null)
        => Meme.Create(CreatedById ?? Guid.NewGuid(), imagePath, text);
}
