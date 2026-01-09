using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Authentication;

namespace SharboAPI.Infrastructure.Services;

public sealed class JwtProvider : IJwtProvider
{
	private readonly HttpClient _httpClient;
	private readonly string _apiKey;
	private readonly string _secureTokenBaseUri;

	public JwtProvider(HttpClient httpClient, IConfiguration configuration)
	{
		_httpClient = httpClient;
		_apiKey = configuration["Authentication:ApiKey"]
		          ?? throw new InvalidOperationException("Authentication:ApiKey not found in configuration.");

		_secureTokenBaseUri = configuration["Authentication:SecureTokenBaseUri"]
		                      ?? throw new InvalidOperationException("Authentication:SecureTokenBaseUri not found in configuration.");
	}

	public async Task<LoginResult> GetForCredentialsAsync(string email, string password, CancellationToken cancellationToken)
	{
		var request = new
		{
			email,
			password,
			returnSecureToken = true
		};

		var response = await _httpClient.PostAsJsonAsync($"accounts:signInWithPassword?key={_apiKey}", request, cancellationToken);
		var authToken = await response.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken);


		return new LoginResult(authToken.AccessToken, authToken.RefreshToken, authToken.ExpiresIn);
	}

	public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
	{
		var refreshUrl = $"{_secureTokenBaseUri}token?key={_apiKey}";

		var request = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("grant_type", "refresh_token"),
			new KeyValuePair<string, string>("refresh_token", refreshToken)
		});

		var response = await _httpClient.PostAsync(refreshUrl, request, cancellationToken);
		var authToken = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken: cancellationToken);

		return new LoginResult(authToken.IdToken, authToken.RefreshToken, authToken.ExpiresIn);
	}
}

public class AuthToken
{
	[JsonPropertyName("idToken")]
	public string AccessToken { get; set; }
	[JsonPropertyName("refreshToken")]
	public string RefreshToken { get; set; }
	[JsonPropertyName("email")]
	public string Email { get; set; }
	[JsonPropertyName("localId")]
	public string UserId { get; set; }
	[JsonPropertyName("registered")]
	public bool Registered { get; set; }
	[JsonPropertyName("expiresIn")]
	public string ExpiresIn { get; set; }
}

public sealed class RefreshTokenResponse
{
	[JsonPropertyName("id_token")]
	public string IdToken { get; set; } = default!;

	[JsonPropertyName("refresh_token")]
	public string RefreshToken { get; set; } = default!;

	[JsonPropertyName("expires_in")]
	public string ExpiresIn { get; set; } = default!;

	[JsonPropertyName("user_id")]
	public string UserId { get; set; } = default!;
}
