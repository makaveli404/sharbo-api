using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.User;

namespace SharboAPI.Application.Services;

public sealed class UserService(IUserRepository userRepository, IAuthenticationService authenticationService, IFirebaseService firebaseService) : IUserService
{
	public async Task<Result<List<UserDetailsDto>>> GetAllAsync(CancellationToken cancellationToken)
	{
		var domainUsers = await userRepository.GetAllAsync(cancellationToken);
		var firebaseUsers = await firebaseService.GetAllAsync([.. domainUsers], cancellationToken);

		var result = new List<UserDetailsDto>();

		foreach (var domainUser in domainUsers)
		{
			var firebaseUser = firebaseUsers.FirstOrDefault(fu => fu.email == domainUser.Email);

			if (string.IsNullOrWhiteSpace(firebaseUser.uid)
			    || string.IsNullOrEmpty(firebaseUser.email) ||
			    string.IsNullOrEmpty(domainUser.Email))
			{
				return Result.Failure<List<UserDetailsDto>>(Error.NotFound("No user found"));
			}

			result.Add(new UserDetailsDto(domainUser.Id.ToString(), domainUser.Email, domainUser.Nickname));
		}

		return Result.Success(result);
	}

	public async Task<Result<UserDetailsDto>> GetByIdAsync(string id, CancellationToken cancellationToken)
	{
		var firebaseUser = await firebaseService.GetByIdAsync(id, cancellationToken);

		if (string.IsNullOrWhiteSpace(firebaseUser.uid))
		{
			return Result.Failure<UserDetailsDto>(Error.NotFound("No user found"));
		}

		var domainUser = await userRepository.GetByEmailAsync(firebaseUser.email, cancellationToken);

		if (string.IsNullOrEmpty(domainUser?.Email))
		{
			return Result.Failure<UserDetailsDto>(Error.NotFound("No user found"));
		}

		var result = new UserDetailsDto(domainUser.Id.ToString(), domainUser.Email, domainUser.Nickname);
		return Result.Success(result);
	}

	public async Task<Result<UserDetailsDto>> GetByEmailAsync(string userEmail, CancellationToken cancellationToken)
	{
		var domainUser = await userRepository.GetByEmailAsync(userEmail, cancellationToken);

		if (string.IsNullOrEmpty(domainUser?.Email))
		{
			return Result.Failure<UserDetailsDto>(Error.NotFound("No user found"));
		}

		var firebaseUser = await firebaseService.GetByEmailAsync(userEmail, cancellationToken);

		if (string.IsNullOrWhiteSpace(firebaseUser.uid) || string.IsNullOrEmpty(firebaseUser.email))
		{
			return Result.Failure<UserDetailsDto>(Error.NotFound("No user found"));
		}

		var result = new UserDetailsDto(domainUser.Id.ToString(), domainUser.Email, domainUser.Nickname);
		return Result.Success(result);
	}

	public async Task<Result<string>> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken)
	{
		var result = await authenticationService.RegisterAsync(nickname, email, password, cancellationToken);

		return result.IsFailure
			? Result.Failure<string>(result.Error)
			: result;
	}

}
