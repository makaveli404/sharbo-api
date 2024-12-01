using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IUserRepository
{
	Task<Guid?> AddAsync(User user, CancellationToken cancellationToken);
}
