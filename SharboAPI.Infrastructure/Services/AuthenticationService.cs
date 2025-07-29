using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;
using SharboAPI.Application.Services;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Services;

public class AuthenticationService(IUserRepository userRepository,
	IFirebaseService firebaseService,
	IServiceProvider serviceProvider) : IAuthenticationService
{
	private readonly SharboDbContext _context = serviceProvider.GetRequiredService<SharboDbContext>();

	public async Task<Result<string>> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken)
	{
		var isUserExist = await firebaseService.IsUserExistAsync(email, cancellationToken);
		if (isUserExist)
		{
			return Result.Failure<string>(Error.Conflict($"User with given e-mail { email } already exists"));
		}

		var isDomainUserExist = await userRepository.IsUserExistByEmailAsync(email, cancellationToken);
		if (isDomainUserExist)
		{
			return Result.Failure<string>(Error.Conflict($"User with given e-mail { email } already exists"));
		}

		await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
		try
		{
			var userId = await firebaseService.RegisterAsync(email, password, cancellationToken);

			var newUser = User.Create(nickname, email, password);
			await userRepository.AddAsync(newUser, cancellationToken);

			await transaction.CommitAsync(cancellationToken);
			return Result.Success(userId);
		}

		catch (Exception ex)
		{
			await transaction.RollbackAsync(cancellationToken);
			throw;
		}
	}
}
