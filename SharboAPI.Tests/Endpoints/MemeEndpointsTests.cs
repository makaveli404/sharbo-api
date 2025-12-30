using SharboAPI.Application.DTO.Meme;
using System.Net;
using System.Net.Http.Json;

namespace SharboAPI.Tests.Endpoints;

public class MemeEndpointsTests : IClassFixture<ApiFactoryFixture>
{
    private readonly HttpClient _client;
    private readonly ApiFactoryFixture _fixture;

    public MemeEndpointsTests(ApiFactoryFixture fixture)
    {
        _fixture = fixture;
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task GetAll_ForNonExistingGroup_ReturnsOkResponseWithEmptyResultArray()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid unexistingGroupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");

        // Act
        var response = await _client.GetAsync($"/api/groups/{unexistingGroupId}/memes/");
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<MemeResult>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ForExistingGroup_ReturnsOkResponseWithAllMemeEntriesContent()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/memes/");
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<MemeResult>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetById_ForNonExistingMeme_ReturnsNotFoundResponse()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid unexistingMemeId = Guid.Parse("b66e8039-9127-4932-a747-17a7cd669733");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/memes/{unexistingMemeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ForExistingMeme_ReturnsOkResponseWithMemeContent()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid memeId = Guid.Parse("b66e8039-9127-4932-a747-17a7cd669733");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/memes/{memeId}");
        var result = await response.Content.ReadFromJsonAsync<MemeResult?>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeOfType<MemeResult?>();
    }

    [Fact]
    public async Task Create_ReturnsBadRequestResponse_WhenMemeCreationFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        CreateMemeRequest request = new(groupId, "image_path", "text");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/groups/{groupId}/memes/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResponse_WhenMemeCreationSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        CreateMemeRequest request = new(groupId, "image_path", "text");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/groups/{groupId}/memes/", request);
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ReturnsBadRequestResponse_WhenMemeUpdateFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid memeId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");
        UpdateMemeRequest request = new("text");

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/groups/{groupId}/memes/{memeId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsNoContentResponse_WhenMemeUpdateSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid memeId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");
        UpdateMemeRequest request = new("text");

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/groups/{groupId}/memes/{memeId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequestResponse_WhenMemeDeletingFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid memeId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");

        // Act
        var response = await _client.DeleteAsync($"/api/groups/{groupId}/memes/{memeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResponse_WhenMemeDeletingSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid memeId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");

        // Act
        var response = await _client.DeleteAsync($"/api/groups/{groupId}/memes/{memeId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
