using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Services;

public interface IGroupService
{
	Task<Group?> GetById(Guid id, CancellationToken cancellationToken);
	Task AddAsync(GroupDto group, CancellationToken cancellationToken);
	Task UpdateAsync(Group group);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
