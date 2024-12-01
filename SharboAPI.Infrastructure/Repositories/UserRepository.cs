using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public class UserRepository(SharboDbContext context) : IUserRepository
{
	public async Task<Guid?> AddAsync(User user, CancellationToken cancellationToken)
	{
		var result = await context.Users.AddAsync(user, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity?.Id;
	}
}
