namespace SharboAPI.Application.Common.Errors;

public sealed class Error(string message, int statusCode, ErrorType errorType)
{
	public string Message { get; init; } = message;
	public int StatusCode { get; init; } = statusCode;
	public ErrorType Type { get; init; } = errorType;

	public static readonly Error None = new(string.Empty, 0, ErrorType.None);
	public static Error BadRequest(string message) => new(message, 400, ErrorType.BadRequest);
	public static Error NotFound(string message) => new(message, 404, ErrorType.NotFound);
	public static Error Unauthorized(string message) => new(message, 401, ErrorType.Unauthorized);
	public static Error Forbidden(string message) => new(message, 403, ErrorType.Forbidden);
	private static Error InternalServerError(string message) => new(message, 500, ErrorType.InternalServerError);
	public static Error Conflict(string message) => new(message, 409, ErrorType.Conflict);
	public static Error ServiceUnavailable(string message) => new(message, 503, ErrorType.ServiceUnavailable);
	public static Error NotImplemented(string message) => new(message, 501, ErrorType.NotImplemented);

	public static Error FromException(Exception ex)
	{
		return ex switch
		{
			_ => InternalServerError("An unexpected error occurred")
		};
	}
}
