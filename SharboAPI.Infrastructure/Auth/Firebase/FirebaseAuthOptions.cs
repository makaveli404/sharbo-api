namespace SharboAPI.Infrastructure.Auth.Firebase;

public sealed class FirebaseAuthOptions
{
	public string ProjectId { get; init; } = null!;
	public string ApiKey { get; init; } = null!;
	public FirebaseEndpointsOptions Endpoints { get; init; } = new();
}

public sealed class FirebaseEndpointsOptions
{
	public string Auth { get; init; } = null!;
	public string RefreshToken { get; init; } = null!;
}
