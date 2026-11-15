using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class UserRepository(SharboDbContext context) : IUserRepository
{
	public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
		=> context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

	public async Task<string> AddAsync(User user, CancellationToken cancellationToken)
	{
		var result = await context.Users.AddAsync(user, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);

		return result.Entity.Id;
	}
}
