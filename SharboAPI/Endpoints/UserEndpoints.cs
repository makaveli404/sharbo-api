using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.User;

namespace SharboAPI.Endpoints;

public static class UserEndpoints
{
	public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
	{
		MapUsersApi(routes);
	}

    private static async Task<IResult> GetAll(IUserService userService, CancellationToken cancellationToken)
	{
		var result = await userService.GetAllAsync(cancellationToken);
		return TypedResults.Ok(result);
	}

	private static async Task<IResult> GetById(Guid id, IUserService userService, CancellationToken cancellationToken)
	{
		var result = await userService.GetByIdAsync(id, cancellationToken);
		return result is not null
			? TypedResults.Ok(result)
			: TypedResults.BadRequest();
	}

	private static async Task<IResult> CreateUser(UserDto user, IUserService userService,
		CancellationToken cancellationToken)
	{
		var result = await userService.AddAsync(user.Nickname, user.Email, user.Password, cancellationToken);
		return result != Guid.Empty
			? TypedResults.CreatedAtRoute(routeName: nameof(GetById), routeValues: new { id = result }, value: user)
			: TypedResults.BadRequest();
	}

	private static void MapUsersApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/users");
		group.MapGet("/", GetAll);
		group.MapGet("/{id}", GetById).WithName(nameof(GetById));
		group.MapPost("/create", CreateUser);
	}
}
