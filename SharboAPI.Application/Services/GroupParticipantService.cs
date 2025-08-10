using Microsoft.Extensions.Logging;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.GroupParticipant;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public class GroupParticipantService(IGroupParticipantRepository groupParticipantRepository,
	IRoleRepository roleRepository, ILogger<GroupParticipantService> logger) : IGroupParticipantService
{
	public async Task<Result<GroupParticipantResult>> GetById(Guid id, CancellationToken cancellationToken)
	{
		var groupParticipant = await groupParticipantRepository.GetByIdAsync(id, cancellationToken);
		if (groupParticipant is null)
		{
			return Result.Failure<GroupParticipantResult>(Error.NotFound("Group participant not found"));
		}

		return Result.Success(new GroupParticipantResult(groupParticipant.Id, groupParticipant.UserId, groupParticipant.GroupParticipantRoles.Select(r => r.Role.RoleType.ToString()).ToList()));
	}

	public async Task<Result<List<GroupParticipantResult>>> GetGroupParticipantsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
	{
		var groupParticipants = await groupParticipantRepository.GetByGroupIdAsync(groupId, cancellationToken);
		if (groupParticipants is null)
		{
			return Result.Failure<List<GroupParticipantResult>>(Error.NotFound("Group participants not found"));
		}

		var groupParticipantResult = groupParticipants.Select(g =>
			new GroupParticipantResult(g.Id, g.UserId, g.GroupParticipantRoles.Select(r => r.Role.RoleType.ToString()).ToList())).ToList();
		return Result.Success(groupParticipantResult);
	}

	public async Task<Result<Guid?>> AddAsync(CreateGroupParticipant createGroupParticipant, Guid groupId, CancellationToken cancellationToken)
	{
		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);

		if (participantRole is null)
		{
			return Result.Failure<Guid?>(Error.NotFound("Participant role not found"));
		}

		var groupParticipantRoles = new List<GroupParticipantRole>
		{
			GroupParticipantRole.Create(participantRole)
		};

		var groupParticipant = GroupParticipant.Create(groupId, createGroupParticipant.UserId, groupParticipantRoles);
		var result = await groupParticipantRepository.AddAsync(groupParticipant, cancellationToken);
		return Result.Success(result);
	}

	public async Task<Result> DeleteAsync(List<Guid> ids, CancellationToken cancellationToken)
	{
		await groupParticipantRepository.DeleteAsync(ids, cancellationToken);
		return Result.Success();
	}

	public async Task<Result> UpdateRolesAsync(Guid participantId, UpdateGroupParticipantRoles updateGroupParticipantRoles, CancellationToken cancellationToken)
	{
		var participant = await groupParticipantRepository.GetByIdAsync(participantId, cancellationToken);
		if (participant is null)
		{
			return Result.Failure(Error.NotFound("Group participant not found"));
		}

		var currentRoles = participant.GroupParticipantRoles.Select(r => r.Role).ToList();

		var rolesToAdd = updateGroupParticipantRoles.RoleTypes.Except(currentRoles.Select(r => r.RoleType)).ToList();
		var rolesToRemove = currentRoles.Select(r => r.RoleType).Except(updateGroupParticipantRoles.RoleTypes).ToList();

		foreach (var roleTypeToAdd in rolesToAdd)
		{
			var role = await roleRepository.GetByRoleTypeAsync(roleTypeToAdd, cancellationToken);
			if (role is null)
			{
				logger.LogWarning("Role with type id {RoleTypeToAdd} not found.", roleTypeToAdd);
				continue;
			}

			participant.GroupParticipantRoles.Add(GroupParticipantRole.Create(role));
		}

		await groupParticipantRepository.DeleteRolesAsync(participantId, rolesToRemove, cancellationToken);

		await groupParticipantRepository.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}
