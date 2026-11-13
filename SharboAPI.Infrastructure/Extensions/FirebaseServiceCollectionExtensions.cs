using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharboAPI.Infrastructure.Options;

namespace SharboAPI.Infrastructure.Extensions;

public static class FirebaseServiceCollectionExtensions
{
	private const string _firebaseAppName = "sharbo";

	/// <summary>
	/// Registers Firebase options and <see cref="FirebaseAuth"/>; the Firebase app itself is created by <see cref="InitializeFirebase"/>
	/// at startup or, if that failed in Development, on first use.
	/// </summary>
	public static IServiceCollection AddFirebase(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddOptions<FirebaseOptions>()
			.Configure(o => configuration.GetSection(FirebaseOptions.SectionName).Bind(o))
			.Validate(o => o.IsConfigured,
				$"Firebase is not configured.");

		services.AddSingleton(serviceProvider =>
		{
			var firebaseOptions = serviceProvider.GetRequiredService<IOptions<FirebaseOptions>>().Value;

			var app = FirebaseApp.GetInstance(_firebaseAppName) ?? FirebaseApp.Create(new AppOptions
			{
				Credential = GoogleCredential.FromJson(firebaseOptions.Credentials),
				ProjectId = firebaseOptions.ProjectId
			}, _firebaseAppName);

			return FirebaseAuth.GetAuth(app);
		});

		return services;
	}

	/// <summary>
	/// Creates <see cref="FirebaseAuth"/> at startup. When <paramref name="required"/>, a missing or invalid key stops the application;
	/// otherwise the problem is only logged and Firebase-dependent endpoints fail on use.
	/// </summary>
	public static void InitializeFirebase(this IServiceProvider services, bool required, ILogger logger)
	{
		try
		{
			services.GetRequiredService<FirebaseAuth>();
		}
		catch (Exception ex) when (!required)
		{
			logger.LogWarning("Firebase is not available: {Reason} The API runs, but user registration and user lookups will fail.",
				ex.Message);
		}
	}
}
