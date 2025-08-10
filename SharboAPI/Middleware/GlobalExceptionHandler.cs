using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharboAPI.Application.Common.Exceptions;

namespace SharboAPI.Middleware;

internal sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
		CancellationToken cancellationToken)
	{
		logger.LogError(exception, "Unhandled exception occurred");

		httpContext.Response.StatusCode = exception switch
		{
			BadHttpRequestException => StatusCodes.Status400BadRequest,
			ApplicationException => StatusCodes.Status400BadRequest,
			FirebaseException => StatusCodes.Status409Conflict,
			ValidationException => StatusCodes.Status409Conflict,
			_ => StatusCodes.Status500InternalServerError
		};

		var problemDetails = new ProblemDetails
		{
			Type = exception.GetType().Name,
			Title = exception.Message,
			Detail = exception.InnerException?.ToString(),
			Status = httpContext.Response.StatusCode
		};

		if (exception is ValidationException validationException)
		{
			problemDetails.Extensions.Add("errors", validationException.Errors);
		}

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = problemDetails
		});
	}
}
