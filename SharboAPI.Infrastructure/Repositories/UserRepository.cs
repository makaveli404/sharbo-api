using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class UserRepository(SharboDbContext context) : IUserRepository
{
	public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
		=> await context.Users.ToListAsync(cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
		=> await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

	public async Task AddAsync(User user, CancellationToken cancellationToken)
	{
		await context.Users.AddAsync(user, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}

    public async Task<bool> IsUserExistByEmailAsync(string email, CancellationToken cancellationToken)
	{
		var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
		return user is not null;
	}
}
