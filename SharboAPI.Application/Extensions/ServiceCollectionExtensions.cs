using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Services;

namespace SharboAPI.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped<IGroupService, GroupService>();
		services.AddScoped<IUserService, UserService>();
		return services;
	}
}
