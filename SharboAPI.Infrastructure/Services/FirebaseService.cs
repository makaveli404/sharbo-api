using SharboAPI.Domain.Models;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Exceptions;
using FirebaseAdmin.Auth;

namespace SharboAPI.Infrastructure.Services;

public sealed class FirebaseService : IFirebaseService
{
    public async Task<List<(string uid, string email)>> GetAllAsync(List<User> domainUsers, CancellationToken cancellationToken)
    {
        try
        {
            List<(string uid, string email)> users = [];
            List<EmailIdentifier> identifiers = domainUsers
                .Select(u => new EmailIdentifier(u.Email))
                .ToList();

            var userResult = await FirebaseAuth.DefaultInstance.GetUsersAsync(identifiers, cancellationToken);

            foreach (var user in userResult.Users)
            {
                users.Add((user.Uid, user.Email));
            }

            return users;
        }

        catch (Exception ex)
        {
            throw new InternalServerErrorException($"An error occured while getting firebase users: { ex.Message }");
        }
    }

    public async Task<(string uid, string email)> GetByIdAsync(string id, CancellationToken cancellation)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(id, cancellation);
            return (userRecord.Uid, userRecord.Email);
        }

        catch (Exception ex)
        {
            if (ex is FirebaseAuthException)
            {
                throw new InternalServerErrorException($"Firebase error has occurred: {ex.Message}");
            }

            throw new InternalServerErrorException($"Unexpected error: {ex.Message}");
        }
    }

    public async Task<(string uid, string email)> GetByEmailAsync(string email, CancellationToken cancellation)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email, cancellation);
            return (userRecord.Uid, userRecord.Email);
        }

        catch (Exception ex)
        {
            if (ex is FirebaseAuthException)
            {
                throw new InternalServerErrorException($"Firebase error has occurred: { ex.Message }");
            }

            throw new InternalServerErrorException($"Unexpected error: { ex.Message }");
        }
    }

    public async Task<string> RegisterAsync(string email, string password, CancellationToken cancellationToken)
    {
        try
        {
            var userArgs = new UserRecordArgs
            {
                Email = email,
                Password = password
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs, cancellationToken);
            return userRecord.Uid;
        }

        catch (Exception ex) 
        {
            if (ex is FirebaseAuthException)
            {
                throw new InternalServerErrorException($"Firebase error has occurred: { ex.Message }");
            }

            throw new InternalServerErrorException($"Unexpected error: { ex.Message }");
        }
    }

    public async Task<bool> IsUserExistAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var userRecord = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email, cancellationToken);
            return userRecord is not null;
        }

        catch (Exception ex)
        {
            if (ex is FirebaseAuthException &&
                ((FirebaseAuthException)ex).AuthErrorCode is AuthErrorCode.UserNotFound)
            {
                return false;
            }

            throw new InternalServerErrorException($"Unexpected error: { ex.Message }");
        }
    }
}
