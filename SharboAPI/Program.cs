using Serilog;
using SharboAPI.Application.Extensions;
using SharboAPI.Extensions;
using SharboAPI.Infrastructure;
using SharboAPI.Infrastructure.Extensions;
using SharboAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddExceptionHandler<GlobalExceptionHandler>()
	.AddProblemDetails()
	.AddWebApiInfrastructure(builder.Configuration)
	.AddInfrastructure(builder.Configuration)
	.AddApplication()
	.AddOpenApi();

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

var app = builder
	.Build()
	.AddWebApiInfrastructure().ApplyMigrationAndSeedAsync<SharboDbContext>();

await app.Result.RunAsync();
