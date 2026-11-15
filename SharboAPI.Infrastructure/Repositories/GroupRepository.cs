using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class GroupRepository(SharboDbContext context) : IGroupRepository
{
	public async Task<Group?> GetById(Guid id, CancellationToken cancellationToken)
		=> await WithParticipants()
			.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

	public async Task<List<Group>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken)
		=> await WithParticipants()
			.Where(g => g.GroupParticipants.Any(p => p.UserId == userId))
			.OrderBy(g => g.Name)
			.ToListAsync(cancellationToken);

	public async Task<Guid?> AddAsync(Group group, CancellationToken cancellationToken)
	{
		var result = await context.Groups.AddAsync(group, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity?.Id;
	}

	public async Task DeleteAsync(Group group, CancellationToken cancellationToken)
	{
		context.Groups.Remove(group);
		await context.SaveChangesAsync(cancellationToken);
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken)
		=> await context.SaveChangesAsync(cancellationToken);

	private IQueryable<Group> WithParticipants()
		=> context.Groups
			.Include(g => g.GroupParticipants)
			.ThenInclude(p => p.GroupParticipantRoles)
			.ThenInclude(r => r.Role);
}
