using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class MemeRepository(SharboDbContext dbContext) : IMemeRepository
{
	public async Task<Meme?> GetById(Guid id, CancellationToken cancellationToken)
		=> await dbContext.Memes.FirstOrDefaultAsync(meme => meme.Id == id, cancellationToken);

	public async Task<Guid?> AddAsync(Meme meme, CancellationToken cancellationToken)
	{
		var result = await dbContext.AddAsync(meme, cancellationToken);
		await SaveChangesAsync(cancellationToken);

		return result.Entity?.Id;
	}

	public async Task DeleteAsync(Meme meme, CancellationToken cancellationToken)
	{
		dbContext.Remove(meme);
		await SaveChangesAsync(cancellationToken);
	}

	public async Task<int> SaveChangesAsync(CancellationToken cancellationToken) 
		=> await dbContext.SaveChangesAsync(cancellationToken);
}
