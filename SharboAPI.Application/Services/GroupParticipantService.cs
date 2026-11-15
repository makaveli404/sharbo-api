using Microsoft.Extensions.Logging;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.GroupParticipant;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class GroupParticipantService(
	IGroupParticipantRepository groupParticipantRepository,
	IRoleRepository roleRepository,
	ICurrentUser currentUser,
	ILogger<GroupParticipantService> logger) : IGroupParticipantService
{
	public async Task<Result<List<GroupParticipantResult>>> GetGroupParticipantsByGroupIdAsync(Guid groupId,
		CancellationToken cancellationToken)
	{
		if (!await IsCurrentUserMemberAsync(groupId, cancellationToken))
		{
			return Result.Failure<List<GroupParticipantResult>>(GroupNotFound(groupId));
		}

		var groupParticipants = await groupParticipantRepository.GetByGroupIdAsync(groupId, cancellationToken) ?? [];
		return Result.Success(groupParticipants.Select(ToResult).ToList());
	}

	public async Task<Result<List<Guid?>>> AddAsync(Guid groupId, string[] userIds, CancellationToken cancellationToken)
	{
		if (!await IsCurrentUserMemberAsync(groupId, cancellationToken))
		{
			return Result.Failure<List<Guid?>>(GroupNotFound(groupId));
		}

		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);
		if (participantRole is null)
		{
			return Result.Failure<List<Guid?>>(Error.NotFound("Participant role not found"));
		}

		var result = new List<Guid?>();

		foreach (var userId in userIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct())
		{
			// Someone who is already in the group is not added a second time.
			if (await groupParticipantRepository.GetByUserIdAndGroupIdAsync(userId, groupId, cancellationToken) is not null)
			{
				continue;
			}

			var groupParticipant = GroupParticipant.Create(groupId, userId, [GroupParticipantRole.Create(participantRole)]);
			result.Add(await groupParticipantRepository.AddAsync(groupParticipant, cancellationToken));
		}

		return Result.Success(result);
	}

	public async Task<Result> DeleteAsync(Guid groupId, Guid[] participantIds, CancellationToken cancellationToken)
	{
		if (!await IsCurrentUserMemberAsync(groupId, cancellationToken))
		{
			return Result.Failure(GroupNotFound(groupId));
		}

		await groupParticipantRepository.DeleteAsync(groupId, participantIds, cancellationToken);
		return Result.Success();
	}

	public async Task<Result> UpdateRolesAsync(Guid groupId, Guid participantId,
		UpdateGroupParticipantRolesRequest updateGroupParticipantRolesRequest, CancellationToken cancellationToken)
	{
		if (!await IsCurrentUserMemberAsync(groupId, cancellationToken))
		{
			return Result.Failure(GroupNotFound(groupId));
		}

		var participant = await groupParticipantRepository.GetByIdAsync(participantId, cancellationToken);
		if (participant is null || participant.GroupId != groupId)
		{
			return Result.Failure(Error.NotFound("Group participant not found"));
		}

		List<Role> requestedRoles = [];

		foreach (var roleName in updateGroupParticipantRolesRequest.Roles)
		{
			var role = await roleRepository.GetByRoleNameAsync(roleName, cancellationToken);

			if (role is null)
			{
				logger.LogWarning("Role with name {RoleName} not found.", roleName);
				return Result.Failure(Error.NotFound("No roles with given names found"));
			}

			requestedRoles.Add(role);
		}

		var currentRoles = participant.GroupParticipantRoles.Select(r => r.Role).ToList();

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

	/// <summary>
	/// False when the current user is not a member of the group or the group does not exist
	/// (both are reported as "not found" so the API does not reveal other groups).
	/// </summary>
	private async Task<bool> IsCurrentUserMemberAsync(Guid groupId, CancellationToken cancellationToken)
		=> await groupParticipantRepository.GetByUserIdAndGroupIdAsync(currentUser.UserId, groupId, cancellationToken) is not null;

	private static Error GroupNotFound(Guid groupId) => Error.NotFound($"No group with ID: { groupId } found");

	private static GroupParticipantResult ToResult(GroupParticipant participant) => new(
		participant.Id,
		participant.UserId,
		participant.GroupParticipantRoles.Select(r => r.Role.RoleType.ToString()).ToList());
}
