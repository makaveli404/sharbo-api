using Microsoft.Extensions.DependencyInjection;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;

namespace SharboAPI.Infrastructure;

public class Seeder(IServiceProvider serviceProvider)
{
    public async Task Seed()
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SharboDbContext>();

        await SeedRoles(dbContext);
    }

    private async Task SeedRoles(SharboDbContext dbContext)
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
