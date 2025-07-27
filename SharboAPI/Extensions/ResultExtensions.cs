using Microsoft.AspNetCore.Mvc;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;

namespace SharboAPI.Extensions;

public static class ResultExtensions
{
	public static IResult ToResult<T>(this Result<T> result)
	{
		if (result.IsSuccess)
		{
			return TypedResults.Ok(result.Value);
		}

		var problemDetails = new ProblemDetails
		{
			Type = $"https://httpstatuses.com/{result.Error.StatusCode}",
			Title = result.Error.Type.ToString(),
			Detail = result.Error.Message,
			Status = result.Error.StatusCode
		};

		return result.Error.Type switch
		{
			ErrorType.BadRequest => TypedResults.BadRequest(problemDetails),
			ErrorType.NotFound => TypedResults.NotFound(problemDetails),
			ErrorType.Forbidden => TypedResults.Forbid(),
			ErrorType.Unauthorized => TypedResults.Unauthorized(),
			ErrorType.Conflict => TypedResults.Conflict(problemDetails),
			_ => TypedResults.Problem(problemDetails)
		};
	}

	public static IResult ToResult(this Result result)
	{
		if (result.IsSuccess)
		{
			return TypedResults.Ok();
		}

		var problemDetails = new ProblemDetails
		{
			Type = $"https://httpstatuses.com/{result.Error.StatusCode}",
			Title = result.Error.Type.ToString(),
			Detail = result.Error.Message,
			Status = result.Error.StatusCode
		};

		return result.Error.Type switch
		{
			ErrorType.BadRequest => TypedResults.BadRequest(problemDetails),
			ErrorType.NotFound => TypedResults.NotFound(problemDetails),
			ErrorType.Forbidden => TypedResults.Forbid(),
			ErrorType.Unauthorized => TypedResults.Unauthorized(),
			ErrorType.Conflict => TypedResults.Conflict(problemDetails),
			_ => TypedResults.Problem(problemDetails)
		};
	}
}
