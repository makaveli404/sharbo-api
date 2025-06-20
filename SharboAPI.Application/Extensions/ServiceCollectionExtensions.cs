using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Services;
using SharboAPI.Application.Validators;

namespace SharboAPI.Application.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddScoped(typeof(IGroupService), typeof(GroupService));
		services.AddScoped(typeof(IUserService),  typeof(UserService));

		services.AddFluentValidationAutoValidation();
		services.AddValidatorsFromAssemblyContaining<GroupValidator>();
		return services;
	}
}
