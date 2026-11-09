using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharboAPI.Infrastructure.Options;

namespace SharboAPI.Infrastructure.Extensions;

public static class FirebaseServiceCollectionExtensions
{
	private const string FirebaseAppName = "sharbo";

	/// <summary>
	/// Registers Firebase without contacting it at startup: the Firebase app is created on first use of <see cref="FirebaseAuth"/>.
	/// With <paramref name="requireAtStartup"/> the app refuses to start when Firebase is not configured.
	/// </summary>
	public static IServiceCollection AddFirebase(this IServiceCollection services, IConfiguration configuration,
		bool requireAtStartup)
	{
		var options = services
			.AddOptions<FirebaseOptions>()
			.Configure(o => configuration.GetSection(FirebaseOptions.SectionName).Bind(o))
			.Validate(o => o.IsConfigured,
				$"Firebase is not configured. Set '{FirebaseOptions.SectionName}:Credentials' (user secrets or the Firebase__Credentials environment variable).");

		if (requireAtStartup)
		{
			options.ValidateOnStart();
		}

		services.AddSingleton(serviceProvider =>
		{
			var firebaseOptions = serviceProvider.GetRequiredService<IOptions<FirebaseOptions>>().Value;

			var app = FirebaseApp.GetInstance(FirebaseAppName) ?? FirebaseApp.Create(new AppOptions
			{
				Credential = GoogleCredential.FromJson(firebaseOptions.Credentials),
				ProjectId = firebaseOptions.ProjectId
			}, FirebaseAppName);

			return FirebaseAuth.GetAuth(app);
		});

		return services;
	}
}
