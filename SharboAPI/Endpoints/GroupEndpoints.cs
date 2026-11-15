using Microsoft.AspNetCore.Mvc;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Application.DTO.GroupParticipant;

namespace SharboAPI.Endpoints;

// Every operation is limited to groups the signed-in user belongs to; other groups answer 404.
public static class GroupEndpoints
{
	public static void MapGroupEndpoints(this IEndpointRouteBuilder routes)
	{
		MapGroupsApi(routes);
	}

	private static async Task<IResult> GetMyGroups(IGroupService groupService, CancellationToken cancellationToken)
	{
		var result = await groupService.GetMineAsync(cancellationToken);
		return TypedResults.Ok(result.Value);
	}

	private static async Task<IResult> GetGroupById(Guid id, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.GetById(id, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok(result.Value);
	}

	private static async Task<IResult> CreateGroup(CreateGroupRequest createGroupRequest, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.AddAsync(createGroupRequest, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.BadRequest();
		}

		return TypedResults.Created($"/api/groups/{result.Value}", result.Value);
	}

	private static async Task<IResult> UpdateGroup(Guid id, UpdateGroupRequest updatedGroupRequest,
		IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.UpdateAsync(id, updatedGroupRequest, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok(result.Value);
	}

	private static async Task<IResult> DeleteGroup(Guid id, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.DeleteAsync(id, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.NoContent();
	}

	private static async Task<IResult> AddParticipants(Guid id,
		[FromQuery] string[] userId, IGroupParticipantService groupParticipantService,
		CancellationToken cancellationToken)
	{
		var result = await groupParticipantService.AddAsync(id, userId, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok(result.Value);
	}

	private static async Task<IResult> RemoveParticipants(Guid id, [FromQuery] Guid[] participantId,
		IGroupParticipantService groupParticipantService, CancellationToken cancellationToken)
	{
		var result = await groupParticipantService.DeleteAsync(id, participantId, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok();
	}

	private static async Task<IResult> UpdateRoles(
		Guid id,
		Guid participantId,
		UpdateGroupParticipantRolesRequest updateGroupParticipantRolesRequest,
		IGroupParticipantService groupParticipantService, CancellationToken cancellationToken)
	{
		var result = await groupParticipantService.UpdateRolesAsync(id, participantId,
			updateGroupParticipantRolesRequest, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.NoContent();
	}

	private static async Task<IResult> GetGroupParticipantsByGroupId(Guid id,
		IGroupParticipantService groupParticipantService, CancellationToken cancellationToken)
	{
		var result = await groupParticipantService.GetGroupParticipantsByGroupIdAsync(id, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok(result.Value);
	}

	private static void MapGroupsApi(this IEndpointRouteBuilder routes)
	{
		var group = routes.MapGroup("/api/groups");

		group.MapGet("/", GetMyGroups);
		group.MapPost("/", CreateGroup);
		group.MapGet("/{id:guid}", GetGroupById);
		group.MapPut("/{id:guid}/update", UpdateGroup);
		group.MapDelete("/{id:guid}", DeleteGroup);
		group.MapGet("/{id:guid}/participants", GetGroupParticipantsByGroupId);
		group.MapPost("/{id:guid}/participants", AddParticipants);
		group.MapDelete("/{id:guid}/participants", RemoveParticipants);
		group.MapPut("/{id:guid}/participants/{participantId:guid}/roles", UpdateRoles);
	}
}
