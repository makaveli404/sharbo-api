using Microsoft.AspNetCore.Mvc;
using SharboAPI.Application.Common;
using SharboAPI.Application.Common.Errors;

namespace SharboAPI.Extensions;

public static class ResultExtensions
{
	public static IResult ToResult<T>(this Result<T> result)
	{
		return result.IsSuccess
			? TypedResults.Ok(result.Value)
			: CreateErrorResult(result.Error);
	}

	public static IResult ToResult(this Result result)
	{
		return result.IsSuccess
			? TypedResults.Ok()
			: CreateErrorResult(result.Error);
	}

	private static IResult CreateErrorResult(Error error)
	{
		var problemDetails = new ProblemDetails
		{
			Type = $"https://httpstatuses.com/{error.StatusCode}",
			Title = error.Type.ToString(),
			Detail = error.Message,
			Status = error.StatusCode
		};

		return error.Type switch
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
