using FirebaseAdmin.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharboAPI.Infrastructure.Extensions;

namespace SharboAPI.Application.Tests.Infrastructure;

public class FirebaseRegistrationTests
{
	private static ServiceProvider BuildProvider(bool requireAtStartup)
	{
		var configuration = new ConfigurationBuilder().Build();

		return new ServiceCollection()
			.AddFirebase(configuration, requireAtStartup)
			.BuildServiceProvider();
	}

	[Fact]
	public void AddFirebase_WithoutCredentialsAndNotRequired_DoesNotBlockStartup()
	{
		// Arrange
		using var provider = BuildProvider(requireAtStartup: false);

		// Act
		var validateStartup = () => provider.GetService<IStartupValidator>()?.Validate();

		// Assert
		validateStartup.Should().NotThrow();
	}

	[Fact]
	public void AddFirebase_WithoutCredentialsAndNotRequired_FailsClearlyOnFirstUse()
	{
		// Arrange
		using var provider = BuildProvider(requireAtStartup: false);

		// Act
		var resolveAuth = () => provider.GetRequiredService<FirebaseAuth>();

		// Assert
		resolveAuth.Should().Throw<OptionsValidationException>()
			.WithMessage("*Firebase is not configured*");
	}

	[Fact]
	public void AddFirebase_WithoutCredentialsAndRequired_BlocksStartup()
	{
		// Arrange
		using var provider = BuildProvider(requireAtStartup: true);

		// Act
		var validateStartup = () => provider.GetRequiredService<IStartupValidator>().Validate();

		// Assert
		validateStartup.Should().Throw<OptionsValidationException>()
			.WithMessage("*Firebase is not configured*");
	}
}
