using SharboAPI.Application.DTO.Authentication;

namespace SharboAPI.Application.Abstractions.Services;

public interface IJwtProvider
{
	Task<LoginResult> GetForCredentialsAsync(string email, string password, CancellationToken cancellationToken);
}
