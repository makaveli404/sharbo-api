using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class GroupRepository(SharboDbContext context) : IGroupRepository
{
	public async Task<Group?> GetById(Guid id, CancellationToken cancellationToken)
	{
		return await context.Groups.Include(g => g.GroupParticipants)
			.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
	}

	public async Task<Guid?> AddAsync(Group group, CancellationToken cancellationToken)
	{
		var result = await context.Groups.AddAsync(group, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity?.Id;
	}

	public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		var group = await GetById(id, cancellationToken);

		if (group is null)
		{
			return;
		}

		context.Groups.Remove(group);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken)
	{
		await context.SaveChangesAsync(cancellationToken);
	}
}
