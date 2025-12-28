using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IQuoteRepository
{
    Task<IEnumerable<Quote>> GetAllByGroupIdAsync(Guid groupId, CancellationToken cancellationToken);
    Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Guid?> AddAsync(Quote quote, CancellationToken cancellationToken);
    Task DeleteAsync(Quote quote, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
