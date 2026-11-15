using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SharboAPI.Application.Extensions;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Endpoints;
using SharboAPI.Extensions;
using Serilog;
using SharboAPI.Infrastructure;
using SharboAPI.Infrastructure.Extensions;
using SharboAPI.Infrastructure.Seeding;
using SharboAPI.Middleware;
using SharboAPI.Services;

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
	builder.Services.AddFirebaseAuthentication(configuration, builder.Environment);
	builder.Services.AddScoped<ICurrentUser, CurrentUser>();
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

		// Lets Swagger UI send a Firebase ID token ("Authorize" button).
		c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Type = SecuritySchemeType.Http,
			Scheme = "bearer",
			BearerFormat = "JWT",
			Description = "Firebase ID token"
		});
		c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
		{
			[new OpenApiSecuritySchemeReference("Bearer", document)] = []
		});
	});

	var app = builder.Build();

	app.UseExceptionHandler();

	// Swagger runs before authentication: the UI and its JSON must load without a token so you can click "Authorize".
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	// Every endpoint below requires a valid Firebase ID token (fallback policy in AddFirebaseAuthentication).
	app.UseAuthentication();
	app.UseAuthorization();

	if (app.Environment.IsDevelopment())
	{
		app.MapOpenApi().AllowAnonymous();
	}

	app.MapGroupEndpoints();
	app.MapMeEndpoints();
	app.MapMemeEndpoints();
	app.MapSituationEndpoints();
	app.MapQuoteEndpoints();

	using var scope = app.Services.CreateScope();
	ApplyMigration<SharboDbContext>(scope);

	var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();
	await seeder.Seed();

	// Demo groups, entries and test-user profiles for local development (turn off with Seed:DevData=false).
	if (app.Environment.IsDevelopment() && configuration.GetValue("Seed:DevData", true))
	{
		await scope.ServiceProvider.GetRequiredService<DevDataSeeder>().SeedAsync();
	}

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
