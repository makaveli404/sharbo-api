using SharboAPI.Infrastructure.Seeding;

namespace SharboAPI.Application.Tests.Persistence;

// Guards the hand-written demo data, so a typo shows up in tests instead of at API startup.
public class DevDataTests
{
	private const int EntryTextMaxLength = 70; // same limit as the create/update entry validators
	private const int NicknameMaxLength = 50;  // same limit as CreateProfileRequestValidator

	private static readonly HashSet<string> Nicknames = DevData.Accounts.Select(a => a.Nickname).ToHashSet();

	[Fact]
	public void Accounts_HaveUniqueUidsEmailsAndValidNicknames()
	{
		// Assert
		DevData.Accounts.Select(a => a.Uid).Should().OnlyHaveUniqueItems();
		DevData.Accounts.Select(a => a.Email).Should().OnlyHaveUniqueItems();
		DevData.Accounts.Select(a => a.Nickname).Should().OnlyHaveUniqueItems()
			.And.AllSatisfy(n => n.Length.Should().BeInRange(1, NicknameMaxLength));
	}

	[Fact]
	public void Groups_HaveUniqueNamesAndOnlyKnownMembers()
	{
		// Assert
		DevData.Groups.Select(g => g.Name).Should().OnlyHaveUniqueItems();

		foreach (var group in DevData.Groups)
		{
			Nicknames.Should().Contain(group.Admin, $"admin of {group.Name} must be a test account");
			group.Members.Should().OnlyHaveUniqueItems().And.NotContain(group.Admin)
				.And.AllSatisfy(m => Nicknames.Should().Contain(m));
			group.Moderators.Should().BeSubsetOf(group.Members);
		}
	}

	[Fact]
	public void Entries_AreWrittenByGroupMembersAndFitValidatorLimits()
	{
		foreach (var group in DevData.Groups)
		{
			var members = group.Members.Append(group.Admin).ToHashSet();

			foreach (var entry in group.Entries)
			{
				members.Should().Contain(entry.Author, $"\"{entry.Text}\" in {group.Name} needs an author from that group");
				entry.Text.Length.Should().BeInRange(1, EntryTextMaxLength, $"\"{entry.Text}\" must fit the validator limit");

				if (entry.Kind == DevData.EntryKind.Meme)
				{
					Uri.IsWellFormedUriString(entry.ImageUrl, UriKind.Absolute).Should().BeTrue();
				}
				else
				{
					entry.ImageSlug.Should().BeNull();
				}
			}
		}
	}

	[Fact]
	public void Groups_EachHaveQuotesSituationsAndMemes()
	{
		// Assert
		DevData.Groups.Should().HaveCount(5);
		DevData.Groups.Should().AllSatisfy(g =>
			g.Entries.Select(e => e.Kind).Distinct().Should().BeEquivalentTo(Enum.GetValues<DevData.EntryKind>()));
	}
}
