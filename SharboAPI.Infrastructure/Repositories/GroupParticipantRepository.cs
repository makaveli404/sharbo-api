using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Enums;
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

	public async Task DeleteAsync(Guid[] id, CancellationToken cancellationToken)
	{
		await context.GroupParticipants.Where(g => id.Contains(g.Id)).ExecuteDeleteAsync(cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteRolesAsync(Guid participantId, List<Role> rolesToRemove, CancellationToken cancellationToken)
		=> 
			await context.GroupParticipantRoles
				.Where(x => x.GroupParticipantId == participantId
							&& rolesToRemove.Contains(x.Role))
				.ExecuteDeleteAsync(cancellationToken);
	
	public async Task SaveChangesAsync(CancellationToken cancellationToken)
	{
		await context.SaveChangesAsync(cancellationToken);
	}
}
