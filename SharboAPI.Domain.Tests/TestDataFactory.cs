namespace SharboAPI.Domain.Tests;

public static class TestDataFactory
{
	public static List<User> CreateUsers(int count = 2)
	{
		var users = new List<User>();
		for (var i = 1; i <= count; i++)
		{
			users.Add(User.Create($"ID{i}", $"User{i}", $"user{i}@example.com", "123456789"));
		}

		return users;
	}

	public static List<GroupParticipant> CreateGroupParticipants(int count = 2)
	{
		var groupParticipants = new List<GroupParticipant>();
		for (var i = 1; i <= count; i++)
		{
			groupParticipants.Add(GroupParticipant.Create(Guid.NewGuid(), Guid.NewGuid()));
		}

		return groupParticipants;
	}
}
