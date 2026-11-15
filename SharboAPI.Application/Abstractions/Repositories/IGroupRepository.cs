using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IGroupRepository
{
	Task<Group?> GetById(Guid id, CancellationToken cancellationToken);
	Task<List<Group>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken);
	Task<Guid?> AddAsync(Group group, CancellationToken cancellationToken);
	Task DeleteAsync(Group group, CancellationToken cancellationToken);
	Task SaveChangesAsync(CancellationToken cancellationToken);
}
