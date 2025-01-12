using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public class UserService(IUserRepository userRepository, IAuthenticationService authenticationService) : IUserService
{
	public Task<Guid> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken)
	{
		var identityId = authenticationService.RegisterAsync(email, password);

		var newUser = User.Create(nickname, email, password);
		return userRepository.AddAsync(newUser, cancellationToken);
	}
}
