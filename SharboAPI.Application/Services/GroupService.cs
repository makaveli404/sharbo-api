using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class GroupService(IGroupRepository groupRepository, IRoleRepository roleRepository) : IGroupService
{
	public async Task<Group?> GetById(Guid id, CancellationToken cancellationToken) 
		=> await groupRepository.GetById(id, cancellationToken);

	public async Task<Guid?> AddAsync(GroupDto groupDto, CancellationToken cancellationToken)
	{
		// Get user id from claim by HttpContextAccessor
		var createdById = Guid.Parse("0B9C7DF2-6829-4316-AA79-A60FAD110E5B");

		// Add creator to group and assign admin role
		var adminRole = await roleRepository.GetByRoleTypeAsync(RoleType.Admin, cancellationToken);
		var moderatorRole = await roleRepository.GetByRoleTypeAsync(RoleType.Moderator, cancellationToken);
		var participantRole = await roleRepository.GetByRoleTypeAsync(RoleType.Participant, cancellationToken);

		var admin = GroupParticipantRole.Create(adminRole);
		var moderator = GroupParticipantRole.Create(adminRole);
		var participant = GroupParticipantRole.Create(adminRole);

		List<GroupParticipant> participants = [
			GroupParticipant.Create(createdById, [admin, moderator, participant])
		];

        // Add participants (if chosen) to group and assign participant role
		if (groupDto.Participants is not null)
		{
			groupDto.Participants.ForEach(dto => 
				participants.Add(GroupParticipant.Create(dto.UserId, [participant]))
			);
		}

        // Create group
        var group = Group.Create(groupDto.Name, createdById, groupDto.ImagePath, participants);

		return await groupRepository.AddAsync(group, cancellationToken);
	}

	public async Task<Group?> UpdateAsync(Guid groupId, UpdateGroupDto updatedGroup, CancellationToken cancellationToken) =>
		await groupRepository.UpdateAsync(groupId, updatedGroup, cancellationToken);

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken) => await groupRepository.DeleteAsync(id, cancellationToken);
}
