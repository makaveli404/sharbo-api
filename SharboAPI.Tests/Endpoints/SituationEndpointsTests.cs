using SharboAPI.Application.DTO.Situation;
using System.Net;
using System.Net.Http.Json;

namespace SharboAPI.Tests.Endpoints;

public class SituationEndpointsTests : IClassFixture<ApiFactoryFixture>
{
    private readonly HttpClient _client;
    private readonly ApiFactoryFixture _fixture;

    public SituationEndpointsTests(ApiFactoryFixture fixture)
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
        var response = await _client.GetAsync($"/api/groups/{unexistingGroupId}/situations/");
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<SituationResult>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ForExistingGroup_ReturnsOkResponseWithAllSituationEntriesContent()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/situations/");
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<SituationResult>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().HaveCountGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetById_ForNonExistingSituation_ReturnsNotFoundResponse()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid unexistingSituationId = Guid.Parse("b66e8039-9127-4932-a747-17a7cd669733");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/situations/{unexistingSituationId}/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ForExistingSituation_ReturnsOkResponseWithSituationContent()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid situationId = Guid.Parse("b66e8039-9127-4932-a747-17a7cd669733");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/situations/{situationId}/");
        var result = await response.Content.ReadFromJsonAsync<SituationResult?>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeOfType<SituationResult?>();
    }

    [Fact]
    public async Task Create_ReturnsBadRequestResponse_WhenSituationCreationFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        CreateSituationRequest request = new(groupId, "text");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/groups/{groupId}/situations/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResponse_WhenSituationCreationSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        CreateSituationRequest request = new(groupId, "text");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/groups/{groupId}/situations/", request);
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ReturnsBadRequestResponse_WhenSituationUpdateFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid situationId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");
        UpdateSituationRequest request = new("text");

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/groups/{groupId}/situations/{situationId}/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsNoContentResponse_WhenSituationUpdateSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid situationId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");
        UpdateSituationRequest request = new("text");

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/groups/{groupId}/situations/{situationId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequestResponse_WhenSituationDeletingFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid situationId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");

        // Act
        var response = await _client.DeleteAsync($"/api/groups/{groupId}/situations/{situationId}/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResponse_WhenSituationDeletingSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid situationId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");

        // Act
        var response = await _client.DeleteAsync($"/api/groups/{groupId}/situations/{situationId}/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
