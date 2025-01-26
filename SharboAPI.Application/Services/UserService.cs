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
		// TODO: Implement pagination, soritng & filtering with Sieve
		var domainUsers = await userRepository.GetAllAsync(cancellationToken);
		var firebaseUsers = await firebaseService.GetAllAsync([.. domainUsers], cancellationToken);

		var users = domainUsers.Select(domainUser =>
		{
            var (Uid, Email) = firebaseUsers.FirstOrDefault(fu => fu.Email == domainUser.Email);

            if (Email is null)
            {
				throw new NotFoundException($"No user with e-mail: { domainUser.Email } found");
            }

			return new UserDetailsDto(domainUser.Id, Uid, Email, domainUser.Nickname);
        });

		return [.. users];
	}

    public async Task<UserDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
		var domainUser = await userRepository
			.GetByIdAsync(id, cancellationToken)
			?? throw new NotFoundException($"No user with ID: { id } found");

		var (Uid, Email) = await firebaseService.GetByEmailAsync(domainUser.Email, cancellationToken);

		return new(domainUser.Id, Uid, Email, domainUser.Nickname);
    }

    public Task<UserDetailsDto> GetById(Guid Id)
    {
        throw new NotImplementedException();
    }

	public async Task<Guid> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken)
	{
		var userId = await authenticationService.RegisterAsync(nickname, email, password, cancellationToken);
		return userId;
	}
}
