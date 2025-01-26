using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Services;

public interface IFirebaseService
{
    Task<List<(string Uid, string Email)>> GetAllAsync(List<User> domainUsers, CancellationToken cancellationToken);
    Task<(string Uid, string Email)> GetByEmailAsync(string email, CancellationToken cancellation);
    Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken);
    Task<bool> IsUserExistAsync(string email, CancellationToken cancellationToken);
}
