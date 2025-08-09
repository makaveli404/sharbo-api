using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.User;
using SharboAPI.Extensions;

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
		return result.ToResult();
	}

	private static async Task<IResult> GetUserById(string id, IUserService userService, CancellationToken cancellationToken)
	{
		var result = await userService.GetByIdAsync(id, cancellationToken);
		return result.ToResult();
	}

	private static async Task<IResult> GetUserByEmail(string email, IUserService userService, CancellationToken cancellationToken)
	{
		var result = await userService.GetByEmailAsync(email, cancellationToken);
		return result.ToResult();
	}

	private static async Task<IResult> CreateUser(CreateUser createUser, IUserService userService,
		CancellationToken cancellationToken)
	{
		var result = await userService.AddAsync(createUser.Nickname, createUser.Email, createUser.Password, cancellationToken);

		return result.IsFailure
			? result.ToResult()
			: TypedResults.CreatedAtRoute(routeName: nameof(GetUserById),
				routeValues: new { id = result.Value },
				value: new { id = result.Value });
	}


	private static void MapUsersApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/user");
		group.MapPost("/create", CreateUser);
		group.MapGet("/", GetAllUsers);
		group.MapGet("/{id}", GetUserById).WithName(nameof(GetUserById));
		group.MapGet("email/{email}", GetUserByEmail).WithName(nameof(GetUserByEmail));
	}
}
