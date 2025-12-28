using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class SituationRepository(SharboDbContext dbContext) : ISituationRepository
{
    public async Task<IEnumerable<Situation>> GetAllByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
        => await dbContext.Situations
            .Where(s => s.CreatedBy.GroupId == groupId)
            .ToListAsync(cancellationToken);

    public async Task<Situation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => await dbContext.Situations.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<Guid?> AddAsync(Situation situation, CancellationToken cancellationToken)
    {
        var result = await dbContext.AddAsync(situation, cancellationToken);
        await SaveChangesAsync(cancellationToken);

        return result.Entity?.Id;
    }

    public async Task DeleteAsync(Situation situation, CancellationToken cancellationToken)
    {
        dbContext.Remove(situation);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        => await dbContext.SaveChangesAsync(cancellationToken);
}
