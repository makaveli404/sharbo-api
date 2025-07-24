namespace SharboAPI.Application.Common;

public class Result
{
	public bool IsSuccess { get; set; }
	public string Error { get; set; }
	public bool IsFailure => !IsSuccess;

	protected Result(bool isSuccess, string error)
	{
		if (isSuccess && error != string.Empty || !isSuccess && error == string.Empty)
		{
			throw new InvalidOperationException();
		}

		IsSuccess = isSuccess;
		Error = error;
	}

	public static Result Success() => new(true, string.Empty);
	public static Result<TValue> Success<TValue>(TValue value) => new(value, true, string.Empty);
	public static Result Failure(string error) => new(false, error);
	public static Result<TValue> Failure<TValue>(string error) => new(default, false, error);
}

public class Result<TValue> : Result
{
	private readonly TValue _value;

	public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access the value of a failure result.");

	protected internal Result(TValue value, bool isSuccess, string error) : base(isSuccess, error)
	{
		_value = value;
	}
}
