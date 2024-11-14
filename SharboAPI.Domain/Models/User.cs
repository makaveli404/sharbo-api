namespace SharboAPI.Domain.Models;

public class User
{
	public Guid Id { get; private set; }
	public string Nickname { get; private set; }
	public string Email { get; private set; }
	public List<GroupParticipants> GroupParticipants { get; private set; } = [];
	public List<Entry> Entries { get; private set; }

	private User() {}

	// Factory methods
	public static User Create(string nickname, string email)
		=> new()
		{
			Id = Guid.NewGuid(),
			Nickname = nickname,
			Email = email
		};
}
