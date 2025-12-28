using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface ISituationRepository
{
    Task<IEnumerable<Situation>> GetAllByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Situation?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid?> AddAsync(Situation situation, CancellationToken cancellationToken);
    Task DeleteAsync(Situation situation, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
