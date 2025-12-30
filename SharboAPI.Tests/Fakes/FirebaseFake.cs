using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Domain.Models;

namespace SharboAPI.Tests.Fakes;

public class FirebaseFake : IFirebaseService
{
    public Task<List<(string uid, string email)>> GetAllAsync(List<User> domainUsers, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<(string uid, string email)> GetByEmailAsync(string email, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public Task<(string uid, string email)> GetByIdAsync(string id, CancellationToken cancellation)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsUserExistAsync(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
