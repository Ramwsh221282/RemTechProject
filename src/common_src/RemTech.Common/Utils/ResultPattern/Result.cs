using Serilog;

namespace RemTechCommon.Utils.ResultPattern;

public sealed record Error(string Description)
{
    public static Error None = new("");
}

public static class ErrorExtensions
{
    public static void LogError(this Error error, ILogger logger) =>
        logger.Error("{Message}", error.Description);

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

    protected Result(Error error) => Error = error;

    public static Result Success() => new Result();

    public static Result Failure(Error error) => new Result(error);

    public static implicit operator Result(Error error) => Failure(error);
}

public sealed class Result<T> : Result
{
    public T Value { get; } = default!;

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
    }

    private Result(Error error)
        : base(error) { }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator T(Result<T> result) => result.Value;

    public static implicit operator Result<T>(Error error) => Failure(error);
}

public static class ResultExtensions
{
    public static Result LogAndReturn(this Result result, ILogger logger, string message)
    {
        if (result.IsFailure)
            return result.Error.LogAndReturn(logger);
        result.Log(logger, message);
        return result;
    }

    public static Result<T> LogAndReturn<T>(this Result<T> result, ILogger logger, string message)
    {
        if (result.IsFailure)
            return result.Error.LogAndReturn(logger);
        result.Log(logger, message);
        return result;
    }

    public static void Log(this Result result, ILogger logger, string message) =>
        logger.Information(message);
}
