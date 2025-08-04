using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Services;

public interface IFirebaseService
{
	Task<List<(string uid, string email)>> GetAllAsync(List<User> domainUsers, CancellationToken cancellationToken);
	Task<(string uid, string email)> GetByIdAsync(string id, CancellationToken cancellation);
	Task<(string uid, string email)> GetByEmailAsync(string email, CancellationToken cancellation);
	Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken);
	Task<bool> IsUserExistAsync(string email, CancellationToken cancellationToken);
}
