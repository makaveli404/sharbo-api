using SharboAPI.Application.DTO.User;

namespace SharboAPI.Application.Abstractions.Services;

public interface IUserService
{
    Task<List<UserDetailsDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserDetailsDto> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<UserDetailsDto> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<string> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
