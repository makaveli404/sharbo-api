using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;

namespace SharboAPI.Endpoints;

public static class GroupEndpoints
{
	public static void MapGroupEndpoints(this IEndpointRouteBuilder routes)
	{
		MapGroupsApi(routes);
	}

	private static async Task<IResult> GetGroupById(Guid id, IGroupService groupService, CancellationToken cancellationToken)
	{
		var result = await groupService.GetById(id, cancellationToken);
		return result is not null
			? TypedResults.Ok(result)
			: TypedResults.NotFound();
	}

	private static async Task<IResult> CreateGroup(GroupDto group, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.AddAsync(group, cancellationToken);
		return result is not null
			? TypedResults.CreatedAtRoute(routeName: nameof(GetGroupById), routeValues: new { id = result }, value: group)
			: TypedResults.BadRequest();
	}

	private static async Task<IResult> UpdateGroup(Guid id, UpdateGroupDto updatedGroup, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.UpdateAsync(id, updatedGroup, cancellationToken);
		return result is not null
			? TypedResults.Ok(result)
			: TypedResults.Ok();
	}

	private static void MapGroupsApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/group");

		group.MapPost("/", CreateGroup);
		group
			.MapGet("/{id:guid}", GetGroupById)
			.WithName(nameof(GetGroupById));
		group.MapPut("/{id:guid}", UpdateGroup);
	}
}
