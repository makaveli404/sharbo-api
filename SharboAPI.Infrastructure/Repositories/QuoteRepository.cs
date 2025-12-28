using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public class QuoteRepository(SharboDbContext dbContext) : IQuoteRepository
{
    public async Task<IEnumerable<Quote>> GetAllByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
        => await dbContext.Quotes
            .Where(q => q.CreatedBy.GroupId == groupId)
            .ToListAsync(cancellationToken);

    public async Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await dbContext.Quotes.FirstOrDefaultAsync(q => q.Id == id, cancellationToken);

    public async Task<Guid?> AddAsync(Quote quote, CancellationToken cancellationToken)
    {
        var result = await dbContext.Quotes.AddAsync(quote, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return result?.Entity.Id;
    }

    public async Task DeleteAsync(Quote quote, CancellationToken cancellationToken)
    {
        dbContext.Remove(quote);
        await SaveChangesAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => dbContext.SaveChangesAsync(cancellationToken);
}
