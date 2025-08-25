using Microsoft.Extensions.Logging;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.GroupParticipant;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class GroupParticipantService(IGroupParticipantRepository groupParticipantRepository,
	IRoleRepository roleRepository, ILogger<GroupParticipantService> logger) : IGroupParticipantService
{
	public async Task<Result<GroupParticipantResult>> GetById(Guid id, CancellationToken cancellationToken)
	{
		var groupParticipant = await groupParticipantRepository.GetByIdAsync(id, cancellationToken);
		if (groupParticipant is null)
		{
			return Result.Failure<GroupParticipantResult>(Error.NotFound("Group participant not found"));
		}

		return Result.Success(new GroupParticipantResult(
			groupParticipant.Id,
			groupParticipant.UserId, 
			groupParticipant.GroupParticipantRoles
				.Select(r => r.Role.RoleType.ToString())
				.ToList()));
	}

	public async Task<Result<List<GroupParticipantResult>>> GetGroupParticipantsByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
	{
		var groupParticipants = await groupParticipantRepository.GetByGroupIdAsync(groupId, cancellationToken);
		if (groupParticipants is null)
		{
			return Result.Failure<List<GroupParticipantResult>>(Error.NotFound("Group participants not found"));
		}

		var groupParticipantResult = groupParticipants.Select(g => new GroupParticipantResult(
				g.Id, 
				g.UserId, 
				g.GroupParticipantRoles
					.Select(r => r.Role.RoleType.ToString())
					.ToList()))
			.ToList();

		return Result.Success(groupParticipantResult);
	}

	public async Task<Result<List<Guid?>>> AddAsync(Guid groupId, string[] userIds, CancellationToken cancellationToken)
	{
		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);

		if (participantRole is null)
		{
			return Result.Failure<List<Guid?>>(Error.NotFound("Participant role not found"));
		}

		var result = new List<Guid?>();

		foreach (var userId in userIds)
		{
			var groupParticipantRoles = new List<GroupParticipantRole>
			{
				GroupParticipantRole.Create(participantRole)
			};

			var groupParticipant = GroupParticipant.Create(groupId, userId, groupParticipantRoles);
			result.Add(await groupParticipantRepository.AddAsync(groupParticipant, cancellationToken));
		}

		return Result.Success(result);
	}

	public async Task<Result> DeleteAsync(Guid[] participantIds, CancellationToken cancellationToken)
	{
		await groupParticipantRepository.DeleteAsync(participantIds, cancellationToken);
		return Result.Success();
	}

	public async Task<Result> UpdateRolesAsync(Guid participantId, 
		UpdateGroupParticipantRolesRequest updateGroupParticipantRolesRequest, CancellationToken cancellationToken)
	{
		var participant = await groupParticipantRepository.GetByIdAsync(participantId, cancellationToken);
		if (participant is null)
		{
			return Result.Failure(Error.NotFound("Group participant not found"));
		}

		List<Role> currentRoles = [];
		List<Role> requestedRoles = [];

		foreach (var participantRole in participant.GroupParticipantRoles)
		{
			currentRoles.Add(participantRole.Role);
		}

		foreach (var roleName in updateGroupParticipantRolesRequest.Roles)
		{
			var role = await roleRepository.GetByRoleNameAsync(roleName, cancellationToken);

			if (role is null)
			{
				logger.LogWarning("Role with name {RoleName} not found.", roleName);
				return Result.Failure<List<Role>>(Error.NotFound("No roles with given names found"));
			}

			requestedRoles.Add(role);
		}

		var rolesToAdd = requestedRoles.Except(currentRoles).ToList();
		var rolesToRemove = currentRoles.Except(requestedRoles).ToList();

		rolesToAdd.ForEach(role => 
			participant.AddRole(GroupParticipantRole.Create(role)));

		rolesToRemove.ForEach(role => 
			participant.RemoveRole(
				participant.GroupParticipantRoles.First(r => r.Role == role)));

		await groupParticipantRepository.SaveChangesAsync(cancellationToken);
		return Result.Success();
	}
}
