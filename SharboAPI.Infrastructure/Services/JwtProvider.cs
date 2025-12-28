using System.Net.Http.Json;
using System.Text.Json.Serialization;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Authentication;

namespace SharboAPI.Infrastructure.Services;

public sealed class JwtProvider : IJwtProvider
{
	private readonly HttpClient _httpClient;

	public JwtProvider(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<LoginResult> GetForCredentialsAsync(string email, string password, CancellationToken cancellationToken)
	{
		var request = new
		{
			email,
			password,
			returnSecureToken = true
		};

		var response = await _httpClient.PostAsJsonAsync("", request, cancellationToken);

		var authToken = await response.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken);

		// handle errors

		return new LoginResult(authToken.AccessToken, authToken.RefreshToken, authToken.ExpiresIn);
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
