using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SharboAPI.Application.Extensions;
using SharboAPI.Endpoints;
using Serilog;
using SharboAPI.Infrastructure;
using SharboAPI.Infrastructure.Extensions;
using SharboAPI.Middleware;

// Bootstrap logger: catches failures that happen before the configured logger is built.
Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateBootstrapLogger();

try
{
	var builder = WebApplication.CreateBuilder(args);

	var configuration = builder.Configuration;

	builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
	builder.Services.AddProblemDetails();

	builder.Services.AddInfrastructure(configuration);
	builder.Services.AddApplication();
	builder.Services.AddOpenApi();

	builder.Host.UseSerilog((context, config) =>
		config.ReadFrom.Configuration(context.Configuration));

	builder.Services.AddControllers()
		.AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
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

	// Outside Development a missing or invalid Firebase key stops startup here, before migrations run.
	app.Services.InitializeFirebase(required: !app.Environment.IsDevelopment(), app.Logger);

	app.UseExceptionHandler();

	app.MapGroupEndpoints();
	app.MapUserEndpoints();
	app.MapMemeEndpoints();
	app.MapSituationEndpoints();
	app.MapQuoteEndpoints();

	if (app.Environment.IsDevelopment())
	{
		app.MapOpenApi();
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	using var scope = app.Services.CreateScope();
	ApplyMigration<SharboDbContext>(scope);

	var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
	await seeder.Seed();

	app.UseSerilogRequestLogging();

	app.UseHttpsRedirection();
	app.MapControllers();
	app.Run();

	return 0;
}
// HostAbortedException is how EF Core tools (dotnet ef) stop the host on purpose; it isn't a failure.
catch (Exception ex) when (ex is not HostAbortedException)
{
	// Log and exit with a non-zero code instead of letting the runtime abort(): as PID 1 in a container
	// an abort is ignored and the process spins at 100% CPU instead of exiting.
	Log.Fatal(ex, "Application failed to start or terminated unexpectedly.");
	return 1;
}
finally
{
	await Log.CloseAndFlushAsync();
}


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
