using System.Text.Json.Serialization;

namespace SharboAPI.Infrastructure.Auth.Firebase;

public sealed class AuthTokenResponse
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
