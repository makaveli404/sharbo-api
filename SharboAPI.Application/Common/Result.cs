using SharboAPI.Application.Common.Errors;

namespace SharboAPI.Application.Common;

public class Result
{
	public bool IsSuccess { get; set; }
	public Error Error { get; set; }
	public bool IsFailure => !IsSuccess;

	protected Result(bool isSuccess, Error error)
	{
		if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
		{
			throw new InvalidOperationException();
		}

		IsSuccess = isSuccess;
		Error = error;
	}

	public static Result Success() => new(isSuccess: true, Error.None);
	public static Result<TValue> Success<TValue>(TValue value) => new(value, isSuccess: true, Error.None);
	public static Result Failure(Error error) => new(isSuccess: false, error);
	public static Result<TValue> Failure<TValue>(Error error) => new(default, isSuccess: false, error);
}

public class Result<TValue> : Result
{
	private readonly TValue _value;

	public TValue Value => IsSuccess 
		? _value 
		: throw new InvalidOperationException("Cannot access the value of a failure result.");

	protected internal Result(TValue value, bool isSuccess, Error error) : base(isSuccess, error)
	{
		_value = value;
	}
}
