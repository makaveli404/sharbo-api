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
		var result = await userService.AddAsync(user.Nickname, user.Email, user.Password, cancellationToken);
		return result != Guid.Empty
			? TypedResults.Created($"{user}/{result}", result)
			: TypedResults.BadRequest();
	}

	private static void MapUsersApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/user");
		group.MapPost("/create", CreateUser);
	}
}
