using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public class GroupService(IGroupRepository groupRepository) : IGroupService
{
	public async Task<Group?> GetById(Guid id, CancellationToken cancellationToken) => await groupRepository.GetById(id, cancellationToken);
	public async Task<Guid?> AddAsync(GroupDto group, CancellationToken cancellationToken)
	{
		// Get user id from claim by HttpContextAccessor
		var createdById = Guid.NewGuid();
		// Temporary definition
		List<GroupParticipants> groupParticipants = [
			GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), true)
		];

		var newGroup = Group.Create(group.Name, createdById, groupParticipants, group.ImagePath);

		return await groupRepository.AddAsync(newGroup, cancellationToken);
	}

	public async Task<Group?> UpdateAsync(Guid groupId, UpdateGroupDto updatedGroup, CancellationToken cancellationToken) =>
		await groupRepository.UpdateAsync(groupId, updatedGroup, cancellationToken);
	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken) => await groupRepository.DeleteAsync(id, cancellationToken);
}
