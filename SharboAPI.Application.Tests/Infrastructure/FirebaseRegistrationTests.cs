using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SharboAPI.Infrastructure.Extensions;

namespace SharboAPI.Application.Tests.Infrastructure;

public class FirebaseRegistrationTests
{
	private static ServiceProvider BuildProvider(string? credentials = null)
	{
		var settings = new Dictionary<string, string?>();
		if (credentials is not null)
		{
			settings["Firebase:Credentials"] = credentials;
		}

		var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();

		return new ServiceCollection()
			.AddFirebase(configuration)
			.BuildServiceProvider();
	}

	[Fact]
	public void InitializeFirebase_WithoutCredentialsAndNotRequired_LogsWarningAndDoesNotThrow()
	{
		// Arrange
		using var provider = BuildProvider();
		var logger = new CapturingLogger();

		// Act
		var initialize = () => provider.InitializeFirebase(required: false, logger);

		// Assert
		initialize.Should().NotThrow();
		logger.Warnings.Should().ContainSingle().Which.Should().Contain("Firebase is not configured");
	}

	[Fact]
	public void InitializeFirebase_WithoutCredentialsAndRequired_BlocksStartup()
	{
		// Arrange
		using var provider = BuildProvider();

		// Act
		var initialize = () => provider.InitializeFirebase(required: true, new CapturingLogger());

		// Assert
		initialize.Should().Throw<OptionsValidationException>()
			.WithMessage("*Firebase is not configured*");
	}

	[Fact]
	public void InitializeFirebase_WithInvalidCredentialsAndRequired_BlocksStartup()
	{
		// Arrange
		using var provider = BuildProvider(credentials: "not-a-service-account-json");

		// Act
		var initialize = () => provider.InitializeFirebase(required: true, new CapturingLogger());

		// Assert
		initialize.Should().Throw<Exception>();
	}

	[Fact]
	public void FirebaseAuth_WithoutCredentials_FailsClearlyOnUse()
	{
		// Arrange
		using var provider = BuildProvider();

		// Act
		var resolveAuth = () => provider.GetRequiredService<FirebaseAuth>();

		// Assert
		resolveAuth.Should().Throw<OptionsValidationException>()
			.WithMessage("*Firebase is not configured*");
	}

	private sealed class CapturingLogger : ILogger
	{
		public List<string> Warnings { get; } = [];

		public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

		public bool IsEnabled(LogLevel logLevel) => true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
			Func<TState, Exception?, string> formatter)
		{
			if (logLevel == LogLevel.Warning)
			{
				Warnings.Add(formatter(state, exception));
			}
		}
	}
}
