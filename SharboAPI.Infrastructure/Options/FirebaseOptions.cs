using Microsoft.Extensions.Configuration;

namespace SharboAPI.Infrastructure.Options;

public sealed class FirebaseOptions
{
	public const string SectionName = "Firebase";

	/// <summary>Service account JSON of the Firebase project.</summary>
	public string? Credentials { get; set; }

	/// <summary>Firebase project id; optional while the service account JSON carries it.</summary>
	public string? ProjectId { get; set; }

	public bool IsConfigured => !string.IsNullOrWhiteSpace(Credentials);

	public static FirebaseOptions From(IConfiguration configuration)
		=> configuration.GetSection(SectionName).Get<FirebaseOptions>() ?? new FirebaseOptions();
}
