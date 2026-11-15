using SharboAPI.Application.Abstractions.Services;

namespace SharboAPI.Services;

/// <summary>
/// Reads the current user from the claims of the verified Firebase ID token.
/// </summary>
public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
	private const string _userIdClaim = "sub";
	private const string _emailClaim = "email";

	public string UserId
	{
		get
		{
			var userId = httpContextAccessor.HttpContext?.User.FindFirst(_userIdClaim)?.Value;

			return string.IsNullOrWhiteSpace(userId)
				? throw new InvalidOperationException(
					"No authenticated user in the current request. Was the endpoint marked AllowAnonymous()?")
				: userId;
		}
	}

	public string? Email
	{
		get
		{
			var email = httpContextAccessor.HttpContext?.User.FindFirst(_emailClaim)?.Value;
			return string.IsNullOrWhiteSpace(email) ? null : email;
		}
	}
}
