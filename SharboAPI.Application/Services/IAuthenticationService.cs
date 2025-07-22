namespace SharboAPI.Application.Services;

public interface IAuthenticationService
{
	Task<string> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
