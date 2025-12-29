using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SharboAPI.Endpoints;
using SharboAPI.Infrastructure.Extensions;

namespace SharboAPI.Extensions;

public static class ServiceCollectionExtensions
{
	public static WebApplication AddWebApiInfrastructure(this WebApplication app)
	{
		app.UseExceptionHandler();
		app.UseSerilogRequestLogging();
		app.UseHttpsRedirection();
		app.UseAuthentication();
		app.UseAuthorization();
		app.RegisterEndpoints();
		app.MapControllers();
		app.ConfigureSwagger();

		return app;
	}

	private static void ConfigureSwagger(this WebApplication app)
	{
		if (!app.Environment.IsDevelopment())
		{
			return;
		}

		app.MapOpenApi();
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	private static void RegisterEndpoints(this WebApplication app)
	{
		// Public
		app.MapAuthEndpoints();

		// Protected
		app.MapGroupEndpoints().RequireAuthorization();
		app.MapUserEndpoints().RequireAuthorization();
		app.MapMemeEndpoints().RequireAuthorization();
		app.MapSituationEndpoints().RequireAuthorization();
		app.MapQuoteEndpoints().RequireAuthorization();
	}

	public static IServiceCollection AddWebApiInfrastructure(
		this IServiceCollection services,
		IConfiguration configuration)
	{
		services.AddControllers()
			.AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
		services.AddEndpointsApiExplorer();
		services.AddDatabaseConfiguration(configuration);

		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new OpenApiInfo
			{
				Title = "Sharbo API",
				Version = "v1",
				Description = "Sharbo API"
			});
		});

		var projectId = configuration["JwtProvider:Firebase:ProjectId"];

		services
			.AddAuthentication("Bearer")
			.AddJwtBearer("Bearer", options =>
			{
				options.Authority = $"https://securetoken.google.com/{projectId}";
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = $"https://securetoken.google.com/{projectId}",
					ValidateAudience = true,
					ValidAudience = projectId,
					ValidateLifetime = true
				};
			});

		services.AddAuthorization();

		return services;
	}
}
