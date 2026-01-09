namespace SharboAPI.Infrastructure.Auth;

public sealed class FirebaseAuthOptions
{
	public string ApiKey { get; init; } = null!;
	public string AuthUri { get; init; } = null!;
	public string RefreshTokenUri { get; init; } = null!;
}
