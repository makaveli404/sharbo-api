using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class GroupService(
	IGroupRepository groupRepository,
	IRoleRepository roleRepository,
	IValidator<CreateGroupDto> createGroupDtoValidator,
	IValidator<UpdateGroupDto> updateGroupDtoValidator) : IGroupService
{
	public async Task<Group?> GetById(Guid id, CancellationToken cancellationToken)
		=> await groupRepository.GetById(id, cancellationToken);

	public async Task<Guid?> AddAsync(CreateGroupDto createGroupDto, CancellationToken cancellationToken)
	{
		await createGroupDtoValidator.ValidateAndThrowAsync(createGroupDto, cancellationToken);

		// TODO: Get user id from claim by HttpContextAccessor insted of creating placeholder manually
		var createdById = Guid.Parse("0B9C7DF2-6829-4316-AA79-A60FAD110E5B");

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
		if (createGroupDto.Participants is not null)
		{
			createGroupDto.Participants.ForEach(dto =>
				participants.Add(GroupParticipant.Create(dto.UserId, [participant]))
			);
		}

		var group = Group.Create(createGroupDto.Name, createdById, createGroupDto.ImagePath, participants);

		return await groupRepository.AddAsync(group, cancellationToken);
	}

	public async Task<Group?> UpdateAsync(Guid groupId, UpdateGroupDto updatedGroup,
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
}
