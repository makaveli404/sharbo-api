using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Services;

namespace SharboAPI.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddHttpContextAccessor();
		services.AddScoped(typeof(IGroupService), typeof(GroupService));
		services.AddScoped(typeof(IUserService),  typeof(UserService));
		services.AddScoped(typeof(IGroupParticipantService), typeof(GroupParticipantService));
		services.AddScoped(typeof(IMemeService), typeof(MemeService));

		services.AddFluentValidationAutoValidation();

		services.AddValidatorsFromAssemblyContaining<UserService>();

		return services;
	}
}
