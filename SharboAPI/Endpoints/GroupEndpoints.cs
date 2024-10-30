using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Endpoints;

public static class GroupEndpoints
{
	public static void MapGroupEndpoints(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/group");

		MapGroupsApi(group);
	}

	private static async Task<IResult> GetGroupById(Guid id, IGroupService groupService, CancellationToken cancellationToken)
	{
		var result = await groupService.GetById(id, cancellationToken);
		return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
	}

	private static async Task<IResult> CreateGroup(GroupDto group, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.AddAsync(group, cancellationToken);
		return result is not null ? TypedResults.Created($"{group}/{result}", result) : TypedResults.BadRequest();
	}

	private static RouteGroupBuilder MapGroupsApi(this RouteGroupBuilder route)
	{
		route.MapGet("/{id:guid}", GetGroupById);
		route.MapPost("/create", CreateGroup);
		return route;
	}
}
