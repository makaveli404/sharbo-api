using Microsoft.EntityFrameworkCore;
using SharboAPI.Application.Abstractions.Repositories;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure.Repositories;

public sealed class RoleRepository(SharboDbContext context) : IRoleRepository
{
    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken cancellationToken) 
        => await context.Roles.ToListAsync(cancellationToken);

    public async Task<Role?> GetByRoleTypeAsync(RoleType roleType, CancellationToken cancellationToken)
        => await context.Roles.FirstOrDefaultAsync(r => r.RoleType == roleType, cancellationToken);   
}
