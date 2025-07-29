using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IUserRepository
{
	Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken);
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
	Task<Guid> AddAsync(User user, CancellationToken cancellationToken);
	Task<bool> IsUserExistByEmailAsync(string email, CancellationToken cancellationToken);
}
