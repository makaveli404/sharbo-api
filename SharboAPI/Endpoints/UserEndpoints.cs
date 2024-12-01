using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.User;

namespace SharboAPI.Endpoints;

public static class UserEndpoints
{
	public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
	{
		MapUsersApi(routes);
	}

	private static async Task<IResult> CreateUser(UserDto user, IUserService userService,
		CancellationToken cancellationToken)
	{
		var result = await userService.AddAsync(user.nickname, user.email, user.password, cancellationToken);
		return result is not null ? TypedResults.Created($"{user}/{result}", result) : TypedResults.BadRequest();
	}

	private static void MapUsersApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/user");

		group.MapPost("/create", CreateUser);
	}
}
