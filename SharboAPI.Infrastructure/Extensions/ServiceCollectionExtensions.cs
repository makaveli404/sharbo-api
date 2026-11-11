using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Services;
using SharboAPI.Infrastructure.Repositories;
using SharboAPI.Infrastructure.Services;

namespace SharboAPI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		var connectionString = configuration.GetConnectionString("SharboDbConnection");

		if (string.IsNullOrWhiteSpace(connectionString))
		{
			throw new InvalidOperationException(
				"Connection string 'SharboDbConnection' is not configured.");
		}

		services.AddDbContext<SharboDbContext>(options =>
			options.UseNpgsql(connectionString,
				npgsql => npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));
		services.AddTransient<Seeder>();

		return services;
	}

	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IGroupRepository, GroupRepository>();
		services.AddScoped<IGroupParticipantRepository, GroupParticipantRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IRoleRepository, RoleRepository>();
		services.AddScoped<IMemeRepository, MemeRepository>();
		services.AddScoped<ISituationRepository, SituationRepository>();
		services.AddScoped<IQuoteRepository, QuoteRepository>();
		services.AddScoped<IFirebaseService, FirebaseService>();
		services.AddScoped<IAuthenticationService, AuthenticationService>();
		services.AddFirebase(configuration);

		return services;
	}
}
