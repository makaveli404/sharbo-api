using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.User;

namespace SharboAPI.Endpoints;

public static class UserEndpoints
{
	public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
	{
		MapUsersApi(routes);
	}

	private static async Task<IResult> GetAllUsers(IUserService userService, CancellationToken cancellationToken)
	{
		var result = await userService.GetAllAsync(cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> GetUserById(string id, IUserService userService, CancellationToken cancellationToken)
	{
		var result = await userService.GetByIdAsync(id, cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> CreateUser(UserDto user, IUserService userService,
		CancellationToken cancellationToken)
	{
		var result = await userService.AddAsync(user.Nickname, user.Email, user.Password, cancellationToken);
		return string.IsNullOrEmpty(result)
			? TypedResults.BadRequest()
			: TypedResults.CreatedAtRoute(routeName: nameof(GetUserById), routeValues: new { id = result }, value: new { id = result });
	}


	private static void MapUsersApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/user");
		group.MapPost("/create", CreateUser);
		group.MapGet("/", GetAllUsers);
		group.MapGet("/{id}", GetUserById).WithName(nameof(GetUserById));
	}
}
