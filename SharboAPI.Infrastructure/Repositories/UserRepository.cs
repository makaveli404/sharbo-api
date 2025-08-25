using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class UserRepository(SharboDbContext context) : IUserRepository
{
	public async Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken)
		=> await context.Users.ToListAsync(cancellationToken);

	public Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
		=> context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
		=> await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

	public async Task<string> AddAsync(User user, CancellationToken cancellationToken)
	{
		var result = await context.Users.AddAsync(user, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);

		return result.Entity.Id;
	}

	public async Task<bool> IsUserExistByEmailAsync(string email, CancellationToken cancellationToken)
	{
		var user = await context.Users.SingleOrDefaultAsync(u => u.Email == email, cancellationToken);
		return user is not null;
	}
}
