namespace SharboAPI.Application.Abstractions.Services;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
