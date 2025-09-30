using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IMemeRepository
{
	Task<IEnumerable<Meme>> GetAllByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
	Task<Meme?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
	Task<Guid?> AddAsync(Meme meme, CancellationToken cancellationToken);
	Task DeleteAsync(Meme meme, CancellationToken cancellationToken);
	Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
