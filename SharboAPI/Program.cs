using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharboAPI.Application.Extensions;
using SharboAPI.Endpoints;
using Serilog;
using SharboAPI.Infrastructure;
using SharboAPI.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddInfrastructure(configuration);
builder.Services.AddApplication();
builder.Services.AddOpenApi();

builder.Host.UseSerilog((context, config) =>
	config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDatabaseConfiguration(configuration);

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Sharbo API",
		Version = "v1",
		Description = "Sharbo API"
	});
});

var app = builder.Build();

app.MapGroupEndpoints();
app.MapUserEndpoints();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sharbo API");
		c.RoutePrefix = string.Empty;
	});
}

using var scope = app.Services.CreateScope();
ApplyMigration<SharboDbContext>(scope);

var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
await seeder.Seed();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.MapControllers();
Log.Information(builder.Configuration["FIREBASE_CREDENTIALS"]?.Substring(0, 100) ?? string.Empty);
app.Run();



static void ApplyMigration<TDbContext>(IServiceScope scope)
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
