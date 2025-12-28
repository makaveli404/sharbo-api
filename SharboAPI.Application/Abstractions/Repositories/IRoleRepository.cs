using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Application.Abstractions.Repositories;

public interface IRoleRepository
{
	Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken);
	Task<Role?> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken);
	Task<Role?> GetByRoleTypeAsync(RoleType roleType, CancellationToken cancellationToken);
}
