using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Services;

public interface IGroupService
{
	Task<Group?> GetById(Guid id, CancellationToken cancellationToken);
	Task<Guid?> AddAsync(GroupDto group, CancellationToken cancellationToken);
	Task<Group?> UpdateAsync(Guid groupId, UpdateGroupDto updatedGroup, CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
