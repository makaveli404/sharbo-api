using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Application.DTO.GroupParticipant;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class GroupService(
	IGroupRepository groupRepository,
	IRoleRepository roleRepository,
	IValidator<CreateGroup> createGroupDtoValidator,
	IValidator<UpdateGroup> updateGroupDtoValidator) : IGroupService
{
	public async Task<GroupResult?> GetById(Guid id, CancellationToken cancellationToken)
	{
		var group = await groupRepository.GetById(id, cancellationToken);
		if (group is null)
		{
			return null;
		}

		var groupResult = new GroupResult(group.Id, group.Name, group.ImagePath, CreateGroupParticipantsResult(group));
		return groupResult;
	}

	public async Task<Guid?> AddAsync(CreateGroup createGroup, CancellationToken cancellationToken)
	{
		await createGroupDtoValidator.ValidateAndThrowAsync(createGroup, cancellationToken);

		// TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
		var createdById = Guid.Parse("3416e059-ca9e-484c-a93a-4816d1db9a10");

		// Add creator to group and assign admin role
		var adminRole = await roleRepository.GetByRoleTypeAsync(RoleType.Admin, cancellationToken);
		var moderatorRole = await roleRepository.GetByRoleTypeAsync(RoleType.Moderator, cancellationToken);
		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);
		ArgumentNullException.ThrowIfNull(adminRole, nameof(RoleType.Admin));
		ArgumentNullException.ThrowIfNull(moderatorRole, nameof(RoleType.Moderator));
		ArgumentNullException.ThrowIfNull(participantRole, nameof(RoleType.Participant));

		var admin = GroupParticipantRole.Create(adminRole!);
		var moderator = GroupParticipantRole.Create(moderatorRole!);
		var participant = GroupParticipantRole.Create(participantRole!);

		List<GroupParticipant> participants =
		[
			GroupParticipant.Create(createdById, [admin, moderator, participant])
		];

		// Add participants (if chosen) to group and assign participant role
		if (createGroup.Participants is not null)
		{
			createGroup.Participants.ForEach(userId =>
				participants.Add(GroupParticipant.Create(userId, [participant]))
			);
		}

		var group = Group.Create(createGroup.Name, createdById, createGroup.ImagePath, participants);

		return await groupRepository.AddAsync(group, cancellationToken);
	}

	public async Task<Group?> UpdateAsync(Guid groupId, UpdateGroup updatedGroup,
		CancellationToken cancellationToken)
	{
		var group = await groupRepository.GetById(groupId, cancellationToken);

		if (group is null)
		{
			return null;
		}

		await updateGroupDtoValidator.ValidateAndThrowAsync(updatedGroup, cancellationToken);

		// TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
		var modifiedBy = Guid.Parse("0B9C7DF2-6829-4316-AA79-A60FAD110E5B");

		group.Update(updatedGroup.Name, modifiedBy, updatedGroup.ImagePath);

		await groupRepository.SaveChangesAsync(cancellationToken);
		return group;
	}


	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		await groupRepository.DeleteAsync(id, cancellationToken);
	}

	private ICollection<GroupParticipantResult> CreateGroupParticipantsResult(Group group)
	{
		var groupParticipantsResult = new List<GroupParticipantResult>();

		foreach (var groupParticipant in group.GroupParticipants)
		{
			groupParticipantsResult.Add(new GroupParticipantResult(groupParticipant.Id, groupParticipant.UserId));
		}

		return groupParticipantsResult;
	}
}
