namespace SharboAPI.Application.Abstractions.Services;

public interface IUserService
{
	Task<Guid?> AddAsync(string nickname, string email, string password, CancellationToken cancellationToken);
}
