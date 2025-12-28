using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.Authentication;

namespace SharboAPI.Application.Abstractions.Services;

public interface IAuthenticationService
{
	Task<Result<string>> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken);
	Task<Result<LoginResult>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken);
	Task<Result<LoginResult>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
