namespace SharboAPI.Application.Abstractions.Services;

public interface IAuthenticationService
{
    Task<Guid> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
