using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Services;

public interface IGroupService
{
	Task<GroupResult?> GetById(Guid id, CancellationToken cancellationToken);
	Task<Guid?> AddAsync(CreateGroup createGroup, CancellationToken cancellationToken);
	Task<Group?> UpdateAsync(Guid groupId, UpdateGroup updatedGroup, CancellationToken cancellationToken);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
