using System.Text.Json.Serialization;

namespace SharboAPI.Infrastructure.Auth.Firebase;

public sealed class RefreshTokenResponse
{
	[JsonPropertyName("id_token")]
	public string IdToken { get; set; } = null!;
	[JsonPropertyName("refresh_token")]
	public string RefreshToken { get; set; } = null!;
	[JsonPropertyName("expires_in")]
	public string ExpiresIn { get; set; } = null!;
	[JsonPropertyName("user_id")]
	public string UserId { get; set; } = null!;
}
