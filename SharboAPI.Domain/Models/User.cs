namespace SharboAPI.Domain.Models;

public class User
{
	public Guid Id { get; private set; }
	public string Nickname { get; private set; }
	public string Email { get; private set; }
	public string Password { get; private set; }
	public List<GroupParticipants> GroupParticipants { get; private set; } = [];
	public List<Entry> Entries { get; private set; }

	private User() {}

	// Factory methods
	public static User Create(string nickname, string email, string password)
	{
		if (string.IsNullOrWhiteSpace(nickname))
		{
			throw new ArgumentNullException(nameof(nickname), "Nickname cannot be null or empty.");
		}

		if (string.IsNullOrWhiteSpace(email))
		{
			throw new ArgumentNullException(nameof(email), "Email cannot be null or empty.");
		}

		return new User
		{
			Id = Guid.NewGuid(),
			Nickname = nickname,
			Email = email,
			Password = password
		};
	}
}
