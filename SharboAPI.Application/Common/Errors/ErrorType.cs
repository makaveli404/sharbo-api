namespace SharboAPI.Application.Common.Errors;

public enum ErrorType
{
	BadRequest,
	Unauthorized,
	Forbidden,
	NotFound,
	Conflict,
	InternalServerError,
	ServiceUnavailable,
	NotImplemented,
	None
}
