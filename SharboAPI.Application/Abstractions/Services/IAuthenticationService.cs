using SharboAPI.Application.Common;

namespace SharboAPI.Application.Abstractions.Services;

public interface IAuthenticationService
{
	Task<Result<string>> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
