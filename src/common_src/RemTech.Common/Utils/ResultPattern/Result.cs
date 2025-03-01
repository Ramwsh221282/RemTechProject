using Serilog;

namespace RemTechCommon.Utils.ResultPattern;

public sealed record Error(string Description)
{
    public static Error None = new("");
}

public static class ErrorExtensions
{
    public static void LogError(this Error error, ILogger logger)
    {
        logger.Error("{Message}", error.Description);
    }

    public static Error LogAndReturn(this Error error, ILogger logger)
    {
        error.LogError(logger);
        return error;
    }
}

public class Result
{
    public bool IsSuccess { get; protected set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    protected Result()
    {
        IsSuccess = true;
        Error = Error.None;
    }

    protected Result(Error error)
    {
        Error = error;
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result Failure(Error error)
    {
        return new Result(error);
    }

    public static implicit operator Result(Error error)
    {
        return Failure(error);
    }
}

public sealed class Result<T> : Result
{
    private readonly T _value = default!;

    public T Value
    {
        get
        {
            if (IsFailure)
                throw new ApplicationException(
                    $"Cannot access body of a failure result. Last error: {Error.Description}"
                );
            return _value;
        }
    }

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
    }

    private Result(Error error)
        : base(error) { }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    private static new Result<T> Failure(Error error)
    {
        return new Result<T>(error);
    }

    public static implicit operator Result<T>(T value)
    {
        return Success(value);
    }

    public static implicit operator T(Result<T> result)
    {
        return result.Value;
    }

    public static implicit operator Result<T>(Error error)
    {
        return Failure(error);
    }
}
