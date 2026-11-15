namespace SharboAPI.Application.Abstractions.Services;

/// <summary>
/// The authenticated user making the current request, read from the verified Firebase ID token.
/// </summary>
public interface ICurrentUser
{
	/// <summary>
	/// Firebase UID of the authenticated user (the same value as <c>Users.Id</c>).
	/// </summary>
	string UserId { get; }

	/// <summary>
	/// E-mail from the token, or null when the sign-in method does not provide one.
	/// </summary>
	string? Email { get; }
}
