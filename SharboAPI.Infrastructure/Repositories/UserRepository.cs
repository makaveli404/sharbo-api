using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class UserRepository(SharboDbContext context) : IUserRepository
{
	public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken)
		=> await context.Users.ToListAsync(cancellationToken);

    public async Task<User?> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
		=> await context.Users.FirstOrDefaultAsync(u => u.Id == Id, cancellationToken);

	public async Task<Guid> AddAsync(User user, CancellationToken cancellationToken)
	{
		var result = await context.Users.AddAsync(user, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
		return result.Entity.Id;
	}

    public async Task<bool> IsUserExistByEmailAsync(string email, CancellationToken cancellationToken)
	{
		var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
		return user is not null;
	}
}
