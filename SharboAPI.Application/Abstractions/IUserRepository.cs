using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions;

public interface IUserRepository
{
	void Add(User user);
}
