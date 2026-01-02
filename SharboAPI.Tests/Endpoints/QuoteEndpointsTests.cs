using SharboAPI.Application.DTO.Quote;
using System.Net;
using System.Net.Http.Json;

namespace SharboAPI.Tests.Endpoints;

public class QuoteEndpointsTests : IClassFixture<ApiFactoryFixture>
{
    private readonly HttpClient _client;
    private readonly ApiFactoryFixture _fixture;

    public QuoteEndpointsTests(ApiFactoryFixture fixture)
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
        var response = await _client.GetAsync($"/api/groups/{unexistingGroupId}/quotes/");
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<QuoteResult>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ForExistingGroup_ReturnsOkResponseWithAllQuoteEntriesContent()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/quotes/");
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<QuoteResult>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task GetById_ForNonExistingQuote_ReturnsNotFoundResponse()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid unexistingQuoteId = Guid.Parse("b66e8039-9127-4932-a747-17a7cd669733");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/quotes/{unexistingQuoteId}/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_ForExistingQuote_ReturnsOkResponseWithQuoteContent()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid quoteId = Guid.Parse("b66e8039-9127-4932-a747-17a7cd669733");

        // Act
        var response = await _client.GetAsync($"/api/groups/{groupId}/quotes/{quoteId}/");
        var result = await response.Content.ReadFromJsonAsync<QuoteResult?>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeOfType<QuoteResult?>();
    }

    [Fact]
    public async Task Create_ReturnsBadRequestResponse_WhenQuoteCreationFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        CreateQuoteRequest request = new(groupId, "text");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/groups/{groupId}/quotes/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_ReturnsCreatedResponse_WhenQuoteCreationSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        CreateQuoteRequest request = new(groupId, "text");

        // Act
        var response = await _client.PostAsJsonAsync($"/api/groups/{groupId}/quotes/", request);
        var result = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Update_ReturnsBadRequestResponse_WhenQuoteUpdateFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid quoteId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");
        UpdateQuoteRequest request = new("text");

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/groups/{groupId}/quotes/{quoteId}/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ReturnsNoContentResponse_WhenQuoteUpdateSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid quoteId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");
        UpdateQuoteRequest request = new("text");

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/groups/{groupId}/quotes/{quoteId}/", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ReturnsBadRequestResponse_WhenQuoteDeletingFailed()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = false;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid quoteId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");

        // Act
        var response = await _client.DeleteAsync($"/api/groups/{groupId}/quotes/{quoteId}/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Delete_ReturnsNoContentResponse_WhenQuoteDeletingSucceded()
    {
        // Arrange
        _fixture.Behavior.IsSuccess = true;
        Guid groupId = Guid.Parse("27c82825-1d93-4210-8a63-d43e3c7c46d4");
        Guid quoteId = Guid.Parse("a638b9e3-d318-4e59-9d25-bc9da29ce9df");

        // Act
        var response = await _client.DeleteAsync($"/api/groups/{groupId}/quotes/{quoteId}/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
