using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Application.Abstractions.Services;
using SharboAPI.Application.Exceptions;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Services;

public sealed class AuthenticationService(IUserRepository userRepository, IFirebaseService firebaseService, 
    IServiceProvider serviceProvider) : IAuthenticationService
{
    private readonly SharboDbContext _context = serviceProvider.GetRequiredService<SharboDbContext>();  

    public async Task<Guid> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken)
    {
        var isUserExist = await firebaseService.IsUserExistAsync(email, cancellationToken);
        if (isUserExist)
        {
            throw new BadRequestException($"User with given e-mail: {email} already exists");
        }

        var isDomainUserExist = await userRepository.IsUserExistByEmailAsync(email, cancellationToken);
        if (isDomainUserExist)
        {
            // TODO: send e-mail about ambiguous failure to administration by email-sending provider
            throw new InternalServerErrorException("Account cannot be created because of user existence");
        }

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await firebaseService.RegisterAsync(email, password, cancellationToken);

            var newUser = User.Create(nickname, email, password);
            var newUserId = await userRepository.AddAsync(newUser, cancellationToken);

            // TODO: send e-mail about account creation to user by email-sending provider

            await transaction.CommitAsync(cancellationToken);
            return newUserId;
        }

        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

}
