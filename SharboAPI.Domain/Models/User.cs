namespace SharboAPI.Domain.Models;

public class User
{
	public string Id { get; private set; }
	public string Nickname { get; private set; }
	public string Email { get; private set; }

	private User() {}

	#region Factory_Methods
	public static User Create(string providerId, string nickname, string email)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(providerId, nameof(providerId));
		ArgumentException.ThrowIfNullOrWhiteSpace(nickname, nameof(nickname));
		ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));

		return new User
		{
			Id = providerId,
			Nickname = nickname,
			Email = email
		};
	}
	#endregion
}
