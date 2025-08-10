using Microsoft.AspNetCore.Mvc;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Application.DTO.GroupParticipant;

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

		if (result.IsFailure)
		{
			return TypedResults.NotFound();
		}

		return TypedResults.Ok(result.Value);
	}

	private static async Task<IResult> CreateGroup(CreateGroup createGroup, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.AddAsync(createGroup, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.BadRequest();
		}

		return TypedResults.Created($"{createGroup}/{result}", result);
	}

	private static async Task<IResult> UpdateGroup(Guid id, UpdateGroup updatedGroup, IGroupService groupService,
		CancellationToken cancellationToken)
	{
		var result = await groupService.UpdateAsync(id, updatedGroup, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.Ok();
		}

		return TypedResults.Ok(result);
	}

	private static async Task<IResult> DeleteGroup(Guid id, IGroupService groupService, CancellationToken cancellationToken)
	{
		await groupService.DeleteAsync(id, cancellationToken);
		return TypedResults.NoContent();
	}

	private static async Task<IResult> AddParticipants(Guid id, CreateGroupParticipant createGroupParticipant, IGroupParticipantService groupParticipantService, CancellationToken cancellationToken)
	{
		var result = await groupParticipantService.AddAsync(createGroupParticipant, id, cancellationToken);
		if (result.IsSuccess)
		{
			return TypedResults.Ok(result);
		}

		return TypedResults.Ok();
	}

	private static async Task<IResult> RemoveParticipants([FromBody] List<Guid> ids, IGroupParticipantService groupParticipantService, CancellationToken cancellationToken)
	{
		await groupParticipantService.DeleteAsync(ids, cancellationToken);
		return TypedResults.Ok();
	}

	private static async Task<IResult> UpdateRoles(Guid participantId, UpdateGroupParticipantRoles updateGroupParticipantRoles,
		IGroupParticipantService groupParticipantService, CancellationToken cancellationToken)
	{
		var result = await groupParticipantService.UpdateRolesAsync(participantId, updateGroupParticipantRoles, cancellationToken);

		if (result.IsFailure)
		{
			return TypedResults.BadRequest();
		}

		return TypedResults.Ok();
	}

	private static async Task<IResult> GetGroupParticipantsByGroupId(Guid id, IGroupParticipantService groupParticipantService, CancellationToken cancellationToken)
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

		group.MapPost("/create", CreateGroup);
		group.MapGet("/{id:guid}", GetGroupById);
		group.MapPut("/{id:guid}/update", UpdateGroup);
		group.MapDelete("/{id:guid}", DeleteGroup);
		group.MapGet("/{id:guid}/participants", GetGroupParticipantsByGroupId);
		group.MapPost("/{id:guid}/add-participants", AddParticipants);
		group.MapPost("/{id:guid}/remove-participants", RemoveParticipants);
		group.MapPost("/{id:guid}/participants/{participantId:guid}/update-roles", UpdateRoles);
	}
}
