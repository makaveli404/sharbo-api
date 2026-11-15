using FluentValidation;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.DTO.User;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public sealed class UserService(
	IUserRepository userRepository,
	IValidator<CreateProfileRequest> createProfileRequestValidator,
	ICurrentUser currentUser) : IUserService
{
	public async Task<Result<UserDetailsResult>> GetCurrentAsync(CancellationToken cancellationToken)
	{
		var user = await userRepository.GetByIdAsync(currentUser.UserId, cancellationToken);

		return user is null
			? Result.Failure<UserDetailsResult>(Error.NotFound("Profile not found"))
			: Result.Success(ToResult(user));
	}

	/// <summary>
	/// Creates the profile after the first sign-in. The ID and e-mail come from the token, only the nickname from the request.
	/// </summary>
	public async Task<Result<UserDetailsResult>> CreateCurrentAsync(CreateProfileRequest request,
		CancellationToken cancellationToken)
	{
		await createProfileRequestValidator.ValidateAndThrowAsync(request, cancellationToken);

		var userId = currentUser.UserId;

		if (await userRepository.GetByIdAsync(userId, cancellationToken) is not null)
		{
			return Result.Failure<UserDetailsResult>(Error.Conflict("Profile already exists"));
		}

		var email = currentUser.Email;
		if (email is null)
		{
			return Result.Failure<UserDetailsResult>(
				Error.BadRequest("The sign-in token has no e-mail address; it is required to create a profile"));
		}

		var user = User.Create(userId, request.Nickname, email);
		await userRepository.AddAsync(user, cancellationToken);

		return Result.Success(ToResult(user));
	}

	private static UserDetailsResult ToResult(User user) => new(user.Id, user.Email, user.Nickname);
}
