using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace SharboAPI.Extensions;

public static class AuthenticationExtensions
{
	/// <summary>
	/// Authenticates requests with Firebase ID tokens (<c>Authorization: Bearer &lt;token&gt;</c>), verified against Google's public keys.
	/// The issuer is <c>Firebase:TokenIssuerBaseUrl</c> (appsettings.json) followed by <c>Firebase:ProjectId</c>.
	/// No Firebase SDK or service-account key is needed: only the public project ID.
	/// Outside Development a missing project ID stops startup; in Development it is only logged and every endpoint returns 401.
	/// All endpoints require authentication by default (fallback policy); public ones must call AllowAnonymous().
	/// </summary>
	public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, IConfiguration configuration,
		IHostEnvironment environment)
	{
		var issuerBaseUrl = configuration["Firebase:TokenIssuerBaseUrl"];
		if (string.IsNullOrWhiteSpace(issuerBaseUrl))
		{
			throw new InvalidOperationException("Firebase:TokenIssuerBaseUrl is not configured (expected in appsettings.json).");
		}

		var issuer = issuerBaseUrl.TrimEnd('/') + "/";
		var projectId = configuration["Firebase:ProjectId"];
		if (string.IsNullOrWhiteSpace(projectId))
		{
			projectId = null;
		}

		if (projectId is null && !environment.IsDevelopment())
		{
			throw new InvalidOperationException(
				"Firebase:ProjectId is not configured.");
		}

		services
			.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				// Keep the original JWT claim names, so the Firebase UID is available as "sub".
				options.MapInboundClaims = false;

				if (projectId is null)
				{
					Log.Warning("Firebase project ID is not configured. Endpoints that require authentication will return 401.");
					options.Events = new JwtBearerEvents
					{
						OnMessageReceived = context =>
						{
							context.NoResult();
							return Task.CompletedTask;
						}
					};
					return;
				}

				options.Authority = issuer + projectId;
				options.TokenValidationParameters.ValidIssuer = issuer + projectId;
				options.TokenValidationParameters.ValidAudience = projectId;

				// Default "Microsoft" log level is Warning, which hides why a token was rejected; log the reason explicitly.
				options.Events = new JwtBearerEvents
				{
					OnAuthenticationFailed = context =>
					{
						Log.Warning("Firebase ID token rejected: {Reason}", context.Exception.Message);
						return Task.CompletedTask;
					},
					OnChallenge = context =>
					{
						if (context.AuthenticateFailure is null)
						{
							var hasHeader = context.Request.Headers.Authorization.Count > 0;
							Log.Warning("401 for {Path}: {Reason}", context.Request.Path,
								hasHeader ? "Authorization header is not a Bearer token" : "no Authorization header");
						}
						return Task.CompletedTask;
					}
				};
			});

		// Every endpoint requires a valid token unless it opts out with AllowAnonymous(),
		// so a newly mapped endpoint can't be left open by accident.
		services.AddAuthorization(options =>
			options.FallbackPolicy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build());

		return services;
	}
}
