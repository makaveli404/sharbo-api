using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public class GroupParticipantRepository(SharboDbContext context) : IGroupParticipantRepository
{
	public async Task<GroupParticipant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return await context.GroupParticipants
			.Include(g => g.GroupParticipantRoles)
			.ThenInclude(g => g.Role)
			.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
	}

	public async Task<List<GroupParticipant>?> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken)
	{
		return await context.GroupParticipants.Include(g => g.GroupParticipantRoles)
			.ThenInclude(r => r.Role)
			.Where(g => g.GroupId == groupId)
			.ToListAsync(cancellationToken);
	}

	public async Task<GroupParticipant?> GetByUserIdAndGroupIdAsync(string userId, Guid groupId, CancellationToken cancellationToken)
	{
		return await context.GroupParticipants
			.Include(g => g.GroupParticipantRoles)
			.ThenInclude(r => r.Role)
			.FirstOrDefaultAsync(g => g.UserId == userId && g.GroupId == groupId, cancellationToken);
	}

	public async Task<Guid?> AddAsync(GroupParticipant groupParticipant, CancellationToken cancellationToken)
	{
		var result = await context.GroupParticipants.AddAsync(groupParticipant, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity?.Id;
	}

	// Only participants of the given group are deleted; IDs from other groups are ignored.
	public async Task DeleteAsync(Guid groupId, Guid[] ids, CancellationToken cancellationToken)
		=> await context.GroupParticipants
			.Where(g => g.GroupId == groupId && ids.Contains(g.Id))
			.ExecuteDeleteAsync(cancellationToken);
	
	public async Task SaveChangesAsync(CancellationToken cancellationToken)
	{
		await context.SaveChangesAsync(cancellationToken);
	}
}
