namespace SharboAPI.Infrastructure.Options;

public sealed class FirebaseOptions
{
	public const string SectionName = "Firebase";
	public string? Credentials { get; init; }
	public string? ProjectId { get; init; }
	public bool IsConfigured => !string.IsNullOrWhiteSpace(Credentials);
}
