using SharboAPI.Domain.Models;

namespace SharboAPI.Domain.Tests;

public static class TestDataFactory
{
	public static List<User> CreateUsers(int count = 2)
	{
		var users = new List<User>();
		for (var i = 1; i <= count; i++)
		{
			users.Add(User.Create($"User{i}", $"user{i}@example.com", "123456789"));
		}

		return users;
	}

	public static List<GroupParticipants> CreateGroupParticipants(int count = 2)
	{
		var groupParticipants = new List<GroupParticipants>();
		for (var i = 1; i <= count; i++)
		{
			groupParticipants.Add(GroupParticipants.Create(Guid.NewGuid(), Guid.NewGuid(), false));
		}

		return groupParticipants;
	}
}
