using FirebaseAdmin.Auth;
using SharboAPI.Application.Abstractions.Services;

namespace SharboAPI.Infrastructure.Authentication;

public class AuthenticationService : IAuthenticationService
{
    public async Task<string> RegisterAsync(string email, string password)
    {
        var userArgs = new UserRecordArgs 
        { 
            Email = email, 
            Password = password
        };

        var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
        return userRecord.Uid;
    }
}
