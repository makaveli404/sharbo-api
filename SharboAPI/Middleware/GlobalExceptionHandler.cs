using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SharboAPI.Application.Common.Exceptions;

namespace SharboAPI.Middleware;

internal sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
		CancellationToken cancellationToken)
	{
		logger.LogError(exception, "Unhandler exception occurred");

		httpContext.Response.StatusCode = exception switch
		{
			ApplicationException => StatusCodes.Status400BadRequest,
			FirebaseException => StatusCodes.Status409Conflict,
			_ => StatusCodes.Status500InternalServerError
		};

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = new ProblemDetails
			{
				Type = exception.GetType().Name,
				Title = "An unexpected error occurred",
				Detail = exception.Message,
				Status = httpContext.Response.StatusCode
			}
		});
	}
}
