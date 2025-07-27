namespace SharboAPI.Application.Common.Errors;

public enum ErrorType
{
	BadRequest = 0,
	Unauthorized = 1,
	Forbidden = 2,
	NotFound = 3,
	Conflict = 4,
	InternalServerError = 5,
	ServiceUnavailable = 6,
	NotImplemented = 7,
	None = 8
}
