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

    public async Task<string> RegisterAsync(string nickname, string email, string password, CancellationToken cancellationToken)
    {
        var isUserExist = await firebaseService.IsUserExistAsync(email, cancellationToken);
        if (isUserExist)
        {
            throw new BadRequestException($"User with given e-mail: { email } already exists");
        }

        var isDomainUserExist = await userRepository.IsUserExistByEmailAsync(email, cancellationToken);
        if (isDomainUserExist)
        {   
            throw new InternalServerErrorException("Account cannot be created because of user existence");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var userId = await firebaseService.RegisterAsync(email, password, cancellationToken);

            var newUser = User.Create(nickname, email);
            await userRepository.AddAsync(newUser, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            return userId;
        }

        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
