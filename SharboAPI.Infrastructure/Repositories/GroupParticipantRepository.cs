using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public class GroupParticipantRepository(SharboDbContext context) : IGroupParticipantRepository
{
	public async Task<GroupParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.GroupParticipants.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
	}

	public async Task<Guid?> AddAsync(GroupParticipant groupParticipant, CancellationToken cancellationToken)
	{
		var result = await context.GroupParticipants.AddAsync(groupParticipant, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity?.Id;
	}

	public async Task DeleteAsync(List<Guid> id, CancellationToken cancellationToken)
	{
		await context.GroupParticipants.Where(g => id.Contains(g.Id)).ExecuteDeleteAsync(cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken)
	{
		await context.SaveChangesAsync(cancellationToken);
	}
}
