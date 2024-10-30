using Microsoft.OpenApi.Models;
using SharboAPI.Application.Extensions;
using SharboAPI.Endpoints;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
		c.RoutePrefix = string.Empty;
	});
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
