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
		var newGroup = new Group
		{
			Id = Guid.NewGuid(),
			Name = group.Name,
			ImagePath = group.ImagePath,
			CreationDate = DateTime.Now
		};

		return await groupRepository.AddAsync(newGroup, cancellationToken);
	}

	public async Task UpdateAsync(Group group) => await groupRepository.UpdateAsync(group);
	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken) => await groupRepository.DeleteAsync(id, cancellationToken);
}
