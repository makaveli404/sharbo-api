using SharboAPI.Application.Common;
using SharboAPI.Application.DTO.User;

namespace SharboAPI.Application.Abstractions.Services;

public interface IUserService
{
    Task<Result<List<UserDetailsDto>>> GetAllAsync(CancellationToken cancellationToken);
    Task<Result<UserDetailsDto>> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<Result<UserDetailsDto>> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Result<string>> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
