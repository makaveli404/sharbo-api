using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IGroupRepository
{
	Task<Group?> GetById(Guid id, CancellationToken cancellationToken);
	Task AddAsync(Group group, CancellationToken cancellationToken);
	Task UpdateAsync(Group group);
	Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
