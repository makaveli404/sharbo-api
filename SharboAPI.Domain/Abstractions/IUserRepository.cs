using SharboAPI.Domain.Models;

namespace SharboAPI.Domain.Abstractions;

public interface IUserRepository
{
	void Add(User user);
}
