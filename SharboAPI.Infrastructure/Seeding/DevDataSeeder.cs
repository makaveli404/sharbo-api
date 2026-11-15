using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharboAPI.Domain.Enums;
using SharboAPI.Domain.Models;
using DomainGroup = SharboAPI.Domain.Models.Group;

namespace SharboAPI.Infrastructure.Seeding;

/// <summary>
/// Fills a local development database with the demo data from <see cref="DevData"/>: profiles of the shared
/// Firebase test accounts, their groups with roles, and quotes, situations and memes.
/// Safe to run on every start: existing profiles are kept and a group is skipped when its admin already has
/// a group with that name.
/// </summary>
public class DevDataSeeder(SharboDbContext dbContext, ILogger<DevDataSeeder> logger)
{
	public async Task SeedAsync(CancellationToken cancellationToken = default)
	{
		var addedUsers = await SeedUsersAsync(cancellationToken);
		var addedGroups = await SeedGroupsAsync(cancellationToken);

		await dbContext.SaveChangesAsync(cancellationToken);

		if (addedUsers > 0 || addedGroups > 0)
		{
			logger.LogInformation("Development data seeded: {Users} test user profiles, {Groups} groups.", addedUsers, addedGroups);
		}
	}

	private async Task<int> SeedUsersAsync(CancellationToken cancellationToken)
	{
		var uids = DevData.Accounts.Select(a => a.Uid).ToList();
		var existing = await dbContext.Users
			.Where(u => uids.Contains(u.Id))
			.Select(u => u.Id)
			.ToListAsync(cancellationToken);

		var missing = DevData.Accounts.Where(a => !existing.Contains(a.Uid)).ToList();
		dbContext.Users.AddRange(missing.Select(a => User.Create(a.Uid, a.Nickname, a.Email)));

		return missing.Count;
	}

	private async Task<int> SeedGroupsAsync(CancellationToken cancellationToken)
	{
		var roles = await dbContext.Roles.ToDictionaryAsync(r => r.RoleType, cancellationToken);
		var uidByNickname = DevData.Accounts.ToDictionary(a => a.Nickname, a => a.Uid);
		var added = 0;

		foreach (var spec in DevData.Groups)
		{
			var adminUid = uidByNickname[spec.Admin];

			if (await dbContext.Groups.AnyAsync(g => g.Name == spec.Name && g.CreatedById == adminUid, cancellationToken))
			{
				continue;
			}

			var participants = new Dictionary<string, GroupParticipant>
			{
				[spec.Admin] = GroupParticipant.Create(adminUid, [
					GroupParticipantRole.Create(roles[RoleType.Admin]),
					GroupParticipantRole.Create(roles[RoleType.Moderator]),
					GroupParticipantRole.Create(roles[RoleType.Participant])
				])
			};

			foreach (var member in spec.Members)
			{
				List<GroupParticipantRole> memberRoles = [GroupParticipantRole.Create(roles[RoleType.Participant])];
				if (spec.Moderators.Contains(member))
				{
					memberRoles.Add(GroupParticipantRole.Create(roles[RoleType.Moderator]));
				}

				participants[member] = GroupParticipant.Create(uidByNickname[member], memberRoles);
			}

			dbContext.Groups.Add(DomainGroup.Create(spec.Name, adminUid, null, participants.Values.ToList()));

			foreach (var entry in spec.Entries)
			{
				var authorId = participants[entry.Author].Id;

				Entry created = entry.Kind switch
				{
					DevData.EntryKind.Quote => Quote.Create(authorId, entry.Text),
					DevData.EntryKind.Situation => Situation.Create(authorId, entry.Text),
					DevData.EntryKind.Meme => Meme.Create(authorId, entry.ImageUrl!, entry.Text),
					_ => throw new ArgumentOutOfRangeException(nameof(entry), entry.Kind, "Unknown entry kind")
				};

				dbContext.Entries.Add(created);
			}

			added++;
		}

		return added;
	}
}
