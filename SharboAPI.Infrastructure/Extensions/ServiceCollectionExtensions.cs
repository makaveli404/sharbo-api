using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Application.Abstractions;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Infrastructure.Repositories;

namespace SharboAPI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
	{
		var databaseProvider = configuration.GetValue<string>("DatabaseProvider");

		switch (databaseProvider)
		{
			case "SQLite":
				services.AddDbContext<SharboDbContext>(options =>
					options.UseSqlite(configuration.GetConnectionString("SharboDbConnection")));
				break;
			case "PostgreSQL":
				services.AddDbContext<SharboDbContext>(options =>
					options.UseNpgsql(configuration.GetConnectionString("SharboDbConnection")));
				break;
			default:
				throw new Exception("Unsupported database provider: " + databaseProvider);
		}

		return services;
	}

	public static IServiceCollection AddInfrastructure(this IServiceCollection services)
	{
		services.AddScoped<IGroupRepository, GroupRepository>();
		return services;
	}
}
