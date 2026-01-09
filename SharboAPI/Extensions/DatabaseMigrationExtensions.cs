using Microsoft.EntityFrameworkCore;
using Serilog;
using SharboAPI.Infrastructure;

namespace SharboAPI.Extensions;

public static class DatabaseMigrationExtensions
{
	public static async Task<WebApplication> ApplyMigrationAndSeedAsync<TDbContext>(this WebApplication app)
		where TDbContext : DbContext
	{
		using var scope = app.Services.CreateScope();
		ApplyMigration<TDbContext>(scope);

		var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
		await seeder.Seed();

		return app;
	}

	private static void ApplyMigration<TDbContext>(IServiceScope scope)
		where TDbContext : DbContext
	{
		try
		{
			Log.Information("Checking if any pending migrations exists.");
			var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
			var pendingMigrations = context.Database.GetPendingMigrations().ToList();
			if (pendingMigrations.Any())
			{
				Log.Information("Applying {Count} pending migrations.", pendingMigrations.Count);
				context.Database.Migrate();
				Log.Information("Finished migrations.");
			}
		}
		catch (Exception ex)
		{
			Log.Error(ex, "Failed to apply migrations for {Name}.", typeof(TDbContext).Name);
			throw;
		}
	}
}
