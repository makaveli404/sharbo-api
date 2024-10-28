using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public class GroupRepository(SharboDbContext context) : IGroupRepository
{
	public async Task<Group?> GetById(Guid id, CancellationToken cancellationToken)
	{
		return await context.Groups.FindAsync(id, cancellationToken);
	}

	public async Task AddAsync(Group group, CancellationToken cancellationToken)
	{
		await context.Groups.AddAsync(group, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task UpdateAsync(Group group)
	{
		context.Groups.Update(group);
		await context.SaveChangesAsync();
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var group = await GetById(id, cancellationToken);
		context.Groups.Remove(group);
		await context.SaveChangesAsync(cancellationToken);
	}
}
