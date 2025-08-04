using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
		Log.Information("Trying to get database provider.");
		var databaseProvider = configuration.GetValue<string>("DatabaseProvider");

		if (string.IsNullOrEmpty(databaseProvider))
		{
			throw new Exception("DatabaseProvider is not configured. Please ensure it exists in appsettings or environment variables.");
		}

		switch (databaseProvider)
		{
			case "SQLite":
				services.AddDbContext<SharboDbContext>(options =>
					options
						.UseSqlite(configuration.GetConnectionString("SharboDbConnection"))
						.ConfigureWarnings(w => w.Ignore(RelationalEventId.NonTransactionalMigrationOperationWarning)));

				break;
			case "PostgreSQL":
				Log.Information("SharboDbConnection: " + configuration.GetConnectionString("SharboDbConnection"));
				services.AddDbContext<SharboDbContext>(options =>
				{
					options.UseNpgsql(configuration.GetConnectionString("SharboDbConnection"))
						.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
				});
				break;
			default:
				throw new Exception("Unsupported database provider: " + databaseProvider);
		}

		services.AddTransient<Seeder>();

		return services;
	}

	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped<IGroupRepository, GroupRepository>();
		services.AddScoped<IUserRepository, UserRepository>();
		services.AddScoped<IRoleRepository, RoleRepository>();
		services.AddScoped<IFirebaseService, FirebaseService>();
		services.AddScoped<IAuthenticationService, AuthenticationService>();

		var json = configuration.GetSection("Firebase:Credentials").Value;

		if (string.IsNullOrWhiteSpace(json))
		{
			throw new InvalidOperationException("Firebase credentials not found in configuration.");
		}

		FirebaseApp.Create(new AppOptions
		{
			Credential = GoogleCredential.FromJson(json)
		});
		return services;
	}
}
