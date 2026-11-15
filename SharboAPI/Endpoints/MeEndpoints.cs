using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.User;
using SharboAPI.Extensions;

namespace SharboAPI.Endpoints;

/// <summary>
/// The signed-in user's own profile. Sign-up and sign-in happen in the client through the Firebase SDK;
/// after the first sign-in the client calls POST /api/me to create the profile.
/// </summary>
public static class MeEndpoints
{
	public static void MapMeEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/me");

		group.MapGet("/", GetProfile).WithName(nameof(GetProfile));
		group.MapPost("/", CreateProfile);
	}

	// 404 means the user is signed in but has no profile yet: the client should ask for a nickname and call POST.
	private static async Task<IResult> GetProfile(IUserService userService, CancellationToken cancellationToken)
	{
		var result = await userService.GetCurrentAsync(cancellationToken);
		return result.ToResult();
	}

	private static async Task<IResult> CreateProfile(CreateProfileRequest request, IUserService userService,
		CancellationToken cancellationToken)
	{
		var result = await userService.CreateCurrentAsync(request, cancellationToken);

		return result.IsFailure
			? result.ToResult()
			: TypedResults.CreatedAtRoute(result.Value, routeName: nameof(GetProfile));
	}
}
