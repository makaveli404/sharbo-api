using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Extensions;

namespace SharboAPI.Endpoints;

public static class AuthEndpoints
{
	public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
	{
		MapAuthApi(routes);
	}

	private static async Task<IResult> Authenticate(string email, string password, IAuthenticationService authenticationService, CancellationToken cancellationToken)
	{
		var result = await authenticationService.AuthenticateAsync(email, password, cancellationToken);
		return result.ToResult();
	}

	private static async Task<IResult> RefreshToken(string refreshToken, IAuthenticationService authenticationService, CancellationToken cancellationToken)
	{
		var result = await authenticationService.RefreshTokenAsync(refreshToken, cancellationToken);
		return result.ToResult();
	}

	private static void MapAuthApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/auth");
		group.MapPost("/", Authenticate);
		group.MapPost("/refresh", RefreshToken);
	}
}
