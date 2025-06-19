using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure;

public class Seeder(SharboDbContext dbContext)
{
    public async Task Seed()
    {
        await SeedRoles();
    }

    private async Task SeedRoles()
    {
        if (dbContext.Roles.Any())
        {
            return;
        }

        Role[] roles = [
            Role.Create(RoleType.Admin, "Admin"),
            Role.Create(RoleType.Moderator, "Moderator"),
            Role.Create(RoleType.Participant, "Participant")
        ];

        await dbContext.Roles.AddRangeAsync(roles);
        await dbContext.SaveChangesAsync();
    }
}
