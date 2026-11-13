using FirebaseAdmin.Auth;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Common.Exceptions;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Services;

public class FirebaseService(FirebaseAuth firebaseAuth) : IFirebaseService
{
	public Task<List<(string uid, string email)>> GetAllAsync(List<User> domainUsers, CancellationToken cancellationToken) =>
		ExecuteAsync(async () =>
		{
			var identifiers = domainUsers
				.Select(u => new EmailIdentifier(u.Email))
				.ToList();

			var userResult = await firebaseAuth.GetUsersAsync(identifiers, cancellationToken);
			return userResult.Users
				.Select(user => (user.Uid, user.Email))
				.ToList();
		});

	public Task<(string uid, string email)> GetByIdAsync(string id, CancellationToken cancellation) =>
		ExecuteAsync(async () =>
		{
			var userRecord = await firebaseAuth.GetUserAsync(id, cancellation);
			return (userRecord.Uid, userRecord.Email);
		});

	public Task<(string uid, string email)> GetByEmailAsync(string email, CancellationToken cancellation) =>
		ExecuteAsync(async () =>
		{
			var userRecord = await firebaseAuth.GetUserByEmailAsync(email, cancellation);
			return (userRecord.Uid, userRecord.Email);
		});

	public Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken) =>
		ExecuteAsync(async () =>
		{
			var userArgs = new UserRecordArgs
			{
				Email = email,
				Password = password
			};

			var userRecord = await firebaseAuth.CreateUserAsync(userArgs, cancellationToken);
			return userRecord.Uid;
		});

	public Task<bool> IsUserExistAsync(string email, CancellationToken cancellationToken) =>
		ExecuteAsync(async () =>
		{
			try
			{
				await firebaseAuth.GetUserByEmailAsync(email, cancellationToken);
				return true;
			}
			catch (FirebaseAuthException ex) when (ex.AuthErrorCode is AuthErrorCode.UserNotFound)
			{
				return false;
			}
		});

	private static async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
	{
		try
		{
			return await action();
		}
		catch (OperationCanceledException)
		{
			throw;
		}
		catch (FirebaseAuthException ex)
		{
			throw new FirebaseException($"Firebase error has occurred: {ex.Message}");
		}
		catch (Exception ex)
		{
			throw new Exception($"Unexpected error: {ex.Message}", ex);
		}
	}
}
