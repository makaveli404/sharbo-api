using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
	public Task<Guid?> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken)
	{
		var newUser = User.Create(nickname, email, password);

		return userRepository.AddAsync(newUser, cancellationToken);
	}
}
