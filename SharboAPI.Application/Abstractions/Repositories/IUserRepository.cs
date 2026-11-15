using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IUserRepository
{
	Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);
	Task<string> AddAsync(User user, CancellationToken cancellationToken);
}
