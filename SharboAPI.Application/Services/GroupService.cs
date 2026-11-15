using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Application.DTO.GroupParticipant;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class GroupService(
	IGroupRepository groupRepository,
	IRoleRepository roleRepository,
	IValidator<CreateGroupRequest> createGroupRequestValidator,
	IValidator<UpdateGroupRequest> updateGroupRequestValidator,
	ICurrentUser currentUser) : IGroupService
{
	public async Task<Result<List<GroupResult>>> GetMineAsync(CancellationToken cancellationToken)
	{
		var groups = await groupRepository.GetAllByUserIdAsync(currentUser.UserId, cancellationToken);
		return Result.Success(groups.Select(ToGroupResult).ToList());
	}

	public async Task<Result<GroupResult?>> GetById(Guid id, CancellationToken cancellationToken)
	{
		var group = await GetGroupOfCurrentUserAsync(id, cancellationToken);

		return group is null
			? Result.Failure<GroupResult?>(GroupNotFound(id))
			: Result.Success<GroupResult?>(ToGroupResult(group));
	}

	public async Task<Result<Guid?>> AddAsync(CreateGroupRequest createGroupRequest, CancellationToken cancellationToken)
	{
		await createGroupRequestValidator.ValidateAndThrowAsync(createGroupRequest, cancellationToken);

		var createdById = currentUser.UserId;

		var adminRole = await roleRepository.GetByRoleTypeAsync(RoleType.Admin, cancellationToken);
		var moderatorRole = await roleRepository.GetByRoleTypeAsync(RoleType.Moderator, cancellationToken);
		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);

		if (adminRole is null || moderatorRole is null || participantRole is null)
		{
			return Result.Failure<Guid?>(Error.NotFound("Roles not found"));
		}

		// The creator joins as Admin; other invited users join as Participants (each once, never the creator twice).
		List<GroupParticipant> participants =
		[
			GroupParticipant.Create(createdById, [
				GroupParticipantRole.Create(adminRole),
				GroupParticipantRole.Create(moderatorRole),
				GroupParticipantRole.Create(participantRole)
			])
		];

		var invitedUserIds = (createGroupRequest.Participants ?? [])
			.Where(userId => !string.IsNullOrWhiteSpace(userId) && userId != createdById)
			.Distinct();

		participants.AddRange(invitedUserIds.Select(userId =>
			GroupParticipant.Create(userId, [GroupParticipantRole.Create(participantRole)])));

		var group = Group.Create(createGroupRequest.Name, createdById, createGroupRequest.ImagePath, participants);

		var result = await groupRepository.AddAsync(group, cancellationToken);
		return Result.Success(result);
	}

	public async Task<Result<GroupResult?>> UpdateAsync(Guid groupId, UpdateGroupRequest updatedGroupRequest,
		CancellationToken cancellationToken)
	{
		await updateGroupRequestValidator.ValidateAndThrowAsync(updatedGroupRequest, cancellationToken);

		var group = await GetGroupOfCurrentUserAsync(groupId, cancellationToken);
		if (group is null)
		{
			return Result.Failure<GroupResult?>(GroupNotFound(groupId));
		}

		group.Update(updatedGroupRequest.Name, currentUser.UserId, updatedGroupRequest.ImagePath);

		await groupRepository.SaveChangesAsync(cancellationToken);
		return Result.Success<GroupResult?>(ToGroupResult(group));
	}

	public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var group = await GetGroupOfCurrentUserAsync(id, cancellationToken);
		if (group is null)
		{
			return Result.Failure(GroupNotFound(id));
		}

		await groupRepository.DeleteAsync(group, cancellationToken);
		return Result.Success();
	}

	/// <summary>
	/// The group, or null when it does not exist or the current user is not its member
	/// (both are reported as "not found" so the API does not reveal other groups).
	/// </summary>
	private async Task<Group?> GetGroupOfCurrentUserAsync(Guid groupId, CancellationToken cancellationToken)
	{
		var group = await groupRepository.GetById(groupId, cancellationToken);
		var userId = currentUser.UserId;

		return group is not null && group.GroupParticipants.Any(p => p.UserId == userId)
			? group
			: null;
	}

	private static Error GroupNotFound(Guid groupId) => Error.NotFound($"No group with ID: { groupId } found");

	private static GroupResult ToGroupResult(Group group) => new(
		group.Id,
		group.Name,
		group.ImagePath,
		group.CreatedById,
		group.GroupParticipants
			.Select(p => new GroupParticipantResult(
				p.Id,
				p.UserId,
				p.GroupParticipantRoles.Select(r => r.Role.RoleType.ToString()).ToList()))
			.ToList());
}
