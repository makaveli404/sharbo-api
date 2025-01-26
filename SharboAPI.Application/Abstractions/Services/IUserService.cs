using SharboAPI.Application.DTO.User;

namespace SharboAPI.Application.Abstractions.Services;

public interface IUserService
{
    Task<List<UserDetailsDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<UserDetailsDto> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<Guid> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
