using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.User;

namespace SharboAPI.Application.Abstractions.Services;

/// <summary>
/// Profile of the signed-in user. Accounts and sign-in live in Firebase; the API only keeps the profile.
/// </summary>
public interface IUserService
{
	Task<Result<UserDetailsResult>> GetCurrentAsync(CancellationToken cancellationToken);
	Task<Result<UserDetailsResult>> CreateCurrentAsync(CreateProfileRequest request, CancellationToken cancellationToken);
}
