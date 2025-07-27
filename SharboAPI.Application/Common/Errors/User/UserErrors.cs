namespace SharboAPI.Application.Common.Errors.User;

public static class UserErrors
{
	public static Error NotFound(string? id = null, string? email = null)
	{
		if (id != null)
		{
			return Error.NotFound($"No user with ID: { id } found");
		}

		if (!string.IsNullOrEmpty(email))
		{
			return Error.NotFound($"No user with e-mail: { email } found");
		}

		return Error.NotFound("No user found");
	}
}
