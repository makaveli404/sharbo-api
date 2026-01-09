using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Authentication;
using SharboAPI.Infrastructure.Auth;
using SharboAPI.Infrastructure.Auth.Firebase;

namespace SharboAPI.Infrastructure.Services;

public sealed class JwtProvider : IJwtProvider
{
	private readonly HttpClient _httpClient;
	private readonly FirebaseAuthOptions _options;

	public JwtProvider(HttpClient httpClient, IOptions<FirebaseAuthOptions> options)
	{
		_httpClient = httpClient;
		_options = options.Value;
	}

	public async Task<LoginResult> GetForCredentialsAsync(string email, string password, CancellationToken cancellationToken)
	{
		var request = new
		{
			email,
			password,
			returnSecureToken = true
		};
		var url = $"{_options.Endpoints.Auth}?key={_options.ApiKey}";

		var response = await _httpClient.PostAsJsonAsync(url, request, cancellationToken);
		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(response.ToString());
		}

		var authToken = await response.Content.ReadFromJsonAsync<AuthTokenResponse>(cancellationToken: cancellationToken);

		if (authToken is null)
		{
			throw new InvalidOperationException("Firebase returned empty response.");
		}

		return new LoginResult(authToken.AccessToken, authToken.RefreshToken, authToken.ExpiresIn);
	}

	public async Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
	{
		var url = $"{_options.Endpoints.RefreshToken}?key={_options.ApiKey}";

		var request = new FormUrlEncodedContent(new[]
		{
			new KeyValuePair<string, string>("grant_type", "refresh_token"),
			new KeyValuePair<string, string>("refresh_token", refreshToken)
		});

		var response = await _httpClient.PostAsync(url, request, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			throw new InvalidOperationException(response.ToString());
		}

		var authToken = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>(cancellationToken: cancellationToken);

		if (authToken is null)
		{
			throw new InvalidOperationException("Firebase returned empty response.");
		}

		return new LoginResult(authToken.IdToken, authToken.RefreshToken, authToken.ExpiresIn);
	}

}
