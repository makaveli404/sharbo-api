using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IMemeRepository
{
    Task<Meme?> GetById(Guid id, CancellationToken cancellationToken);
    Task<Guid?> AddAsync(Meme meme, CancellationToken cancellationToken);
    Task DeleteAsync(Meme meme, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
