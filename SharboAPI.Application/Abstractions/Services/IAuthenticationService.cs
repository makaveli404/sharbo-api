namespace SharboAPI.Application.Abstractions.Services;

public interface IAuthenticationService
{
    Task<string> RegisterAsync(string email, string password);
}
