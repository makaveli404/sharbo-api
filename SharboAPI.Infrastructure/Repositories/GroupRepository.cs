using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.DTO.Group;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class GroupRepository(SharboDbContext context) : IGroupRepository
{
	public async Task<Group?> GetById(Guid id, CancellationToken cancellationToken)
	{
		return await context.Groups.FindAsync(id, cancellationToken);
	}

	public async Task<Guid?> AddAsync(Group group, CancellationToken cancellationToken)
	{
		var result = await context.Groups.AddAsync(group, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity?.Id;
	}

	public async Task<Group?> UpdateAsync(Guid groupId, UpdateGroupDto updatedGroup, CancellationToken cancellationToken)
	{
        var group = await context.Groups.FindAsync(groupId, cancellationToken);

		if (group is null)
		{
			return null;
		}

		group.Update(updatedGroup.Name, updatedGroup.ModifiedById, updatedGroup.ImagePath);

		var result = context.Groups.Update(group);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity;
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
}
