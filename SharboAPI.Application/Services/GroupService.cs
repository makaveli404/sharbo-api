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
	IValidator<CreateGroupRequest> createGroupDtoValidator,
	IValidator<UpdateGroupRequest> updateGroupDtoValidator) : IGroupService
{
	public async Task<Result<GroupResult?>> GetById(Guid id, CancellationToken cancellationToken)
	{
		var group = await groupRepository.GetById(id, cancellationToken);
		if (group is null)
		{
			return Result.Failure<GroupResult?>(Error.NotFound("Group not found"));
		}

		var groupResult = new GroupResult(group.Id, group.Name, group.ImagePath, group.CreatedById, CreateGroupParticipantsResult(group));
		return Result.Success<GroupResult?>(groupResult);
	}

	public async Task<Result<Guid?>> AddAsync(CreateGroupRequest createGroupRequest, CancellationToken cancellationToken)
	{
		await createGroupDtoValidator.ValidateAndThrowAsync(createGroupRequest, cancellationToken);

		// TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
		var createdById = Guid.Parse("3416e059-ca9e-484c-a93a-4816d1db9a10");

		// Add creator to group and assign admin role
		var adminRole = await roleRepository.GetByRoleTypeAsync(RoleType.Admin, cancellationToken);
		var moderatorRole = await roleRepository.GetByRoleTypeAsync(RoleType.Moderator, cancellationToken);
		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);

		if (adminRole is null || moderatorRole is null || participantRole is null)
		{
			return Result.Failure<Guid?>(Error.NotFound("Roles not found"));
		}

		var admin = GroupParticipantRole.Create(adminRole!);
		var moderator = GroupParticipantRole.Create(moderatorRole!);
		var participant = GroupParticipantRole.Create(participantRole!);

		List<GroupParticipant> participants =
		[
			GroupParticipant.Create(createdById, [admin, moderator, participant])
		];

		// Add participants (if chosen) to group and assign participant role
		if (createGroupRequest.Participants is not null)
		{
			createGroupRequest.Participants.ForEach(userId =>
				participants.Add(GroupParticipant.Create(userId, [participant]))
			);
		}

		var group = Group.Create(createGroupRequest.Name, createdById, createGroupRequest.ImagePath, participants);

		var result = await groupRepository.AddAsync(group, cancellationToken);
		return Result.Success(result);
	}

	public async Task<Result<GroupResult?>> UpdateAsync(Guid groupId, UpdateGroupRequest updatedGroupRequest,
		CancellationToken cancellationToken)
	{
		var group = await groupRepository.GetById(groupId, cancellationToken);

		if (group is null)
		{
			return Result.Failure<GroupResult?>(Error.NotFound("Group not found"));
		}

		await updateGroupDtoValidator.ValidateAndThrowAsync(updatedGroupRequest, cancellationToken);

		// TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
		var modifiedBy = Guid.Parse("0B9C7DF2-6829-4316-AA79-A60FAD110E5B");

		group.Update(updatedGroupRequest.Name, modifiedBy, updatedGroupRequest.ImagePath);

		await groupRepository.SaveChangesAsync(cancellationToken);

		var groupResult = new GroupResult(group.Id, group.Name, group.ImagePath, group.CreatedById, CreateGroupParticipantsResult(group));
		return Result.Success<GroupResult?>(groupResult);
	}


	public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		await groupRepository.DeleteAsync(id, cancellationToken);
		return Result.Success();
	}

	private ICollection<GroupParticipantResult> CreateGroupParticipantsResult(Group group)
	{
		var groupParticipantsResult = new List<GroupParticipantResult>();

		foreach (var groupParticipant in group.GroupParticipants)
		{
			groupParticipantsResult.Add(new GroupParticipantResult(groupParticipant.Id, groupParticipant.UserId, groupParticipant.GroupParticipantRoles.Select(r => r.Role.RoleType.ToString()).ToList()));
		}

		return groupParticipantsResult;
	}
}
