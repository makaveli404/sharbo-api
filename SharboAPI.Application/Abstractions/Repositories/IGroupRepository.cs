using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IGroupRepository
{
	Task<Group?> GetById(Guid id, CancellationToken cancellationToken);
	Task<Guid?> AddAsync(Group group, CancellationToken cancellationToken);
	Task<Group?> UpdateAsync(Guid groupId, UpdateGroupDto updatedGroup, CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
