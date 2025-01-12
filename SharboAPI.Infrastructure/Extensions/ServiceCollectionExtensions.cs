using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Infrastructure.Repositories;

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

		return services;
	}

	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddScoped<IGroupRepository, GroupRepository>();
		services.AddScoped<IUserRepository, UserRepository>();

		FirebaseApp.Create(new AppOptions
		{
			Credential = GoogleCredential.FromFile("firebase.json")
		});

		return services;
	}
}
