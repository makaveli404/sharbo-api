using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.DTO.User;
using SharboAPI.Application.Exceptions;

namespace SharboAPI.Application.Services;

public sealed class UserService(IAuthenticationService authenticationService, IFirebaseService firebaseService, 
	IUserRepository userRepository) : IUserService
{
	public async Task<List<UserDetailsDto>> GetAllAsync(CancellationToken cancellationToken)
	{
		var domainUsers = await userRepository.GetAllAsync(cancellationToken);
		var firebaseUsers = await firebaseService.GetAllAsync([.. domainUsers], cancellationToken);

		var users = domainUsers.Select(domainUser =>
		{
            var (uid, email) = firebaseUsers.FirstOrDefault(fu => fu.email == domainUser.Email);

            if (email is null)
            {
				throw new NotFoundException($"No user with e-mail: { domainUser.Email } found");
            }

			return new UserDetailsDto(uid, email, domainUser.Nickname);
        });

		return [.. users];
	}

    public async Task<UserDetailsDto> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
		var (uid, email) = await firebaseService.GetByIdAsync(id, cancellationToken);

		if(email is null)
		{
			throw new NotFoundException($"No user with ID: { id } found");
		}

		var domainUser = await userRepository
			.GetByEmailAsync(email, cancellationToken)
			?? throw new NotFoundException($"No user with e-mail: { email } found");

        return new(uid, email, domainUser.Nickname);
    }

    public async Task<UserDetailsDto> GetByEmailAsync(string userEmail, CancellationToken cancellationToken)
    {
        var domainUser = await userRepository
            .GetByEmailAsync(userEmail, cancellationToken)
            ?? throw new NotFoundException($"No user with e-mail: {userEmail} found");

        var (uid, email) = await firebaseService.GetByEmailAsync(userEmail, cancellationToken);

        return new(uid, email, domainUser.Nickname);
    }

    public async Task<string> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken)
	{
		var userId = await authenticationService.RegisterAsync(nickname, email, password, cancellationToken);
		return userId;
	}
}
